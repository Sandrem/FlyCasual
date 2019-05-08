using Ship;
using System;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class HarpoonMissiles : GenericSpecialWeapon
    {
        public HarpoonMissiles() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Harpoon Missiles",
                UpgradeType.Missile,
                cost: 4,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 4,
                    minRange: 2,
                    maxRange: 3,
                    requiresToken: typeof(BlueTargetLockToken),
                    discard: true
                ),
                abilityType: typeof(Abilities.FirstEdition.HarpoonMissilesAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class HarpoonMissilesAbility : GenericAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            IsAppliesConditionCard = true;
        }

        public override void ActivateAbility()
        {
            HostShip.OnShotHitAsAttacker += PlanToApplyHarpoonMissilesCondition;
        }

        public override void DeactivateAbility()
        {
            // Ability is turned off only after full attack is finished
            HostShip.OnCombatDeactivation += DeactivateAbilityPlanned;
        }

        private void DeactivateAbilityPlanned(GenericShip ship)
        {
            HostShip.OnCombatDeactivation -= DeactivateAbilityPlanned;
            HostShip.OnShotHitAsAttacker -= PlanToApplyHarpoonMissilesCondition;
        }

        private void PlanToApplyHarpoonMissilesCondition()
        {
            if (Combat.ChosenWeapon == this.HostUpgrade)
            {
                HostShip.OnAttackFinishAsAttacker += ApplyHarpoonMissilesCondition;

                //Missile was discarded
                HostShip.OnShotHitAsAttacker -= PlanToApplyHarpoonMissilesCondition;
            }
        }

        private void ApplyHarpoonMissilesCondition(GenericShip attacker)
        {
            HostShip.OnAttackFinishAsAttacker -= ApplyHarpoonMissilesCondition;

            Messages.ShowInfo("The \"Harpooned!\" condition has been assigned to " + Combat.Defender.PilotInfo.PilotName);
            Combat.Defender.Tokens.AssignCondition(typeof(Conditions.Harpooned));
        }
    }
}

namespace ActionsList
{

    public class HarpoonedRepairAction : GenericAction
    {
        public HarpoonedRepairAction()
        {
            Name = DiceModificationName = "\"Harpooned!\": Discard condition";
        }

        public override void ActionTake()
        {
            HostShip.Tokens.RemoveCondition(typeof(Conditions.Harpooned));

            Phases.StartTemporarySubPhaseOld(
                "Damage from \"Harpooned!\" condition",
                typeof(SubPhases.HarpoonMissilesCheckSubPhase),
                delegate {
                    Phases.FinishSubPhase(typeof(SubPhases.HarpoonMissilesCheckSubPhase));
                    Phases.CurrentSubPhase.CallBack();
                });
        }

        public override int GetActionPriority()
        {
            int result = 90;

            return result;
        }

    }

}

namespace Conditions
{
    public class Harpooned : Tokens.GenericToken
    {
        public Harpooned(GenericShip host) : base(host)
        {
            Name = "Harpooned Condition";
            Temporary = false;
            Tooltip = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/conditions/harpooned.png";
        }

        public override void WhenAssigned()
        {
            SubscribeToHarpoonedConditionEffects();
        }

        public override void WhenRemoved()
        {
            UnsubscribeFromHarpoonedConditionEffects();
        }

        private void SubscribeToHarpoonedConditionEffects()
        {
            Host.OnShotHitAsDefender += CheckUncancelledCrit;
            Host.OnShipIsDestroyed += DoSplashDamageOnDestroyed;
            Host.OnGenerateActions += AddRepairAction;
        }

        private void UnsubscribeFromHarpoonedConditionEffects()
        {
            Host.OnShotHitAsDefender -= CheckUncancelledCrit;
            Host.OnShipIsDestroyed -= DoSplashDamageOnDestroyed;
            Host.OnGenerateActions -= AddRepairAction;
        }

        private void CheckUncancelledCrit()
        {
            if (Combat.DiceRollAttack.CriticalSuccesses > 0)
            {
                Triggers.RegisterTrigger(
                    new Trigger()
                    {
                        Name = "Harpooned!: Critical hit is dealt",
                        TriggerType = TriggerTypes.OnAttackHit,
                        TriggerOwner = Host.Owner.PlayerNo,
                        EventHandler = HarpoonDetonationByCrit
                    }
                );
            }
        }

        private void HarpoonDetonationByCrit(object sender, System.EventArgs e)
        {
            DoSplashDamage(Host, AdditionalDamageOnItself);
        }

        private void DoSplashDamage(GenericShip harpoonedShip, Action callback)
        {
            Messages.ShowInfo("\"Harpooned!\" condition deals splash damage");

            var ships = Roster.AllShips.Select(x => x.Value).ToList();

            foreach (GenericShip ship in ships)
            {

                // Defending ship shouldn't suffer additional damage
                if (ship.ShipId == harpoonedShip.ShipId)
                {
                    continue;
                }

                BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(harpoonedShip, ship);

                if (distanceInfo.Range == 1)
                {
                    DamageSourceEventArgs harpoonconditionDamage = new DamageSourceEventArgs()
                    {
                        Source = "Harpoon Condition",
                        DamageType = DamageTypes.CardAbility
                    };

                    ship.Damage.TryResolveDamage(1, harpoonconditionDamage, callback);
                }
            }
        }

        private void AdditionalDamageOnItself()
        {
            Host.Tokens.RemoveCondition(this);

            DamageDecks.GetDamageDeck(Host.Owner.PlayerNo).DrawDamageCard(
                false,
                DealDrawnCard,
                new DamageSourceEventArgs
                {
                    DamageType = DamageTypes.Rules,
                    Source = null
                }
            );
        }

        private void DealDrawnCard(System.EventArgs e)
        {
            Host.Damage.DealDrawnCard(Triggers.FinishTrigger);
        }

        private void DoSplashDamageOnDestroyed(GenericShip harpoonedShip, bool isFled)
        {
            Triggers.RegisterTrigger(
                new Trigger()
                {
                    Name = "Harpooned!: Ship is destroyed",
                    TriggerType = TriggerTypes.OnShipIsDestroyed,
                    TriggerOwner = Host.Owner.PlayerNo,
                    EventHandler = HarpoonDetonationByDestruction
                }
            );
        }

        private void HarpoonDetonationByDestruction(object sender, System.EventArgs e)
        {
            DoSplashDamage(Host, Triggers.FinishTrigger);
        }

        private void AddRepairAction(GenericShip harpoonedShip)
        {
            ActionsList.GenericAction action = new ActionsList.HarpoonedRepairAction()
            {
                ImageUrl = (new Harpooned(harpoonedShip)).Tooltip,
                HostShip = harpoonedShip
            };
            harpoonedShip.AddAvailableAction(action);
        }
    }
}

namespace SubPhases
{

    public class HarpoonMissilesCheckSubPhase : DiceRollCheckSubPhase
    {

        public override void Prepare()
        {
            DiceKind = DiceKind.Attack;
            DiceCount = 1;

            AfterRoll = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            if (CurrentDiceRoll.DiceList[0].Side == DieSide.Success || CurrentDiceRoll.DiceList[0].Side == DieSide.Crit)
            {
                DamageSourceEventArgs harpoonconditionDamage = new DamageSourceEventArgs()
                {
                    Source = "Harpoon Condition",
                    DamageType = DamageTypes.CardAbility
                };

                Selection.ThisShip.Damage.TryResolveDamage(1, harpoonconditionDamage, Triggers.FinishTrigger);
            }
            else
            {
                CallBack();
            }
        }

    }

}