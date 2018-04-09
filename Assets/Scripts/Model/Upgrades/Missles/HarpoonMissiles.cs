using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;
using Abilities;
using Ship;

namespace UpgradesList
{
    public class HarpoonMissiles : GenericSecondaryWeapon
    {
        public HarpoonMissiles() : base()
        {
            Types.Add(UpgradeType.Missile);

            Name = "Harpoon Missiles";

            Cost = 4;
            MinRange = 2;
            MaxRange = 3;
            AttackValue = 4;

            RequiresTargetLockOnTargetToShoot = true;
            IsDiscardedForShot = true;

            UpgradeAbilities.Add(new HarpoonMissilesAbility());
        }

    }
}

namespace Abilities
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
            //No deactivation is required
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

            Messages.ShowInfo("\"Harpooned!\" condition is assigned");
            Combat.Defender.Tokens.AssignCondition(new Conditions.Harpooned(Combat.Defender));
        }
    }
}

namespace ActionsList
{

    public class HarpoonedRepairAction : GenericAction
    {
        public HarpoonedRepairAction()
        {
            Name = EffectName = "\"Harpooned!\": Discard condition";
        }

        public override void ActionTake()
        {
            Host.Tokens.RemoveCondition(typeof(Conditions.Harpooned));

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
            Host.AfterGenerateAvailableActionsList += AddRepairAction;
        }

        private void UnsubscribeFromHarpoonedConditionEffects()
        {
            Host.OnShotHitAsDefender -= CheckUncancelledCrit;
            Host.OnShipIsDestroyed -= DoSplashDamageOnDestroyed;
            Host.AfterGenerateAvailableActionsList -= AddRepairAction;
        }

        private void CheckUncancelledCrit()
        {
            if (Combat.DiceRollAttack.CriticalSuccesses > 0)
            {
                Triggers.RegisterTrigger(
                    new Trigger() {
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

                Board.ShipDistanceInformation distanceInfo = new Board.ShipDistanceInformation(harpoonedShip, ship);

                if (distanceInfo.Range == 1)
                {
                    var diceRoll = new DiceRoll(DiceKind.Attack, 0, DiceRollCheckType.Combat);
                    diceRoll.AddDice(DieSide.Success);
                    var hitDie = diceRoll.DiceList[0];
                    ship.AssignedDamageDiceroll.DiceList.Add(hitDie);

                    Triggers.RegisterTrigger(new Trigger()
                    {
                        Name = "Suffer \"Harpooned!\" condition damage",
                        TriggerType = TriggerTypes.OnDamageIsDealt,
                        TriggerOwner = ship.Owner.PlayerNo,
                        EventHandler = ship.SufferDamage,
                        Skippable = true,
                        EventArgs = new DamageSourceEventArgs()
                        {
                            Source = "\"Harpooned!\" condition",
                            DamageType = DamageTypes.CardAbility
                        }
                    });
                }
            }

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, callback);
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
                    Name = "Harpooned!: Ship is destryed",
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
                Host = harpoonedShip
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
            diceType = DiceKind.Attack;
            diceCount = 1;

            finishAction = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            if (CurrentDiceRoll.DiceList[0].Side == DieSide.Success || CurrentDiceRoll.DiceList[0].Side == DieSide.Crit)
            {
                Selection.ThisShip.AssignedDamageDiceroll.AddDice(DieSide.Success);

                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Suffer \"Harpooned!\" condition damage",
                    TriggerType = TriggerTypes.OnDamageIsDealt,
                    TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                    EventHandler = Selection.ThisShip.SufferDamage,
                    EventArgs = new DamageSourceEventArgs()
                    {
                        Source = "\"Harpooned!\" condition ",
                        DamageType = DamageTypes.CardAbility
                    }
                });

                Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, CallBack);
            }
            else
            {
                CallBack();
            }
        }

    }

}