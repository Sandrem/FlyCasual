using Upgrade;
using Ship;
using System.Linq;
using Abilities;
using RuleSets;
using ActionsList;

namespace UpgradesList
{
    public class SaturationSalvo : GenericUpgrade, ISecondEditionUpgrade
    {
        public SaturationSalvo() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Saturation Salvo";
            Cost = 1;

            UpgradeAbilities.Add(new SaturationSalvoAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 6;
            SEImageNumber = 14;
            UpgradeAbilities.Clear();
            UpgradeAbilities.Add(new Abilities.SecondEdition.SaturationSalvoAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            if (RuleSet.Instance is SecondEdition)
            {
                return ship.ActionBar.HasAction(typeof(ReloadAction));
            }
            else
            {
                return true;
            }
        }
    }
}

namespace Abilities
{
    public class SaturationSalvoAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker += RegisterSaturationSalvoAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker -= RegisterSaturationSalvoAbility;
        }

        private void RegisterSaturationSalvoAbility()
        {
            GenericSecondaryWeapon weapon = Combat.ChosenWeapon as GenericSecondaryWeapon;
            if (weapon != null)
            {
                if (weapon.HasType(UpgradeType.Torpedo) || weapon.HasType(UpgradeType.Missile))
                {
                    Triggers.RegisterTrigger(
                        new Trigger()
                        {
                            Name = "Saturation Salvo",
                            TriggerType = TriggerTypes.OnAttackMissed,
                            TriggerOwner = HostShip.Owner.PlayerNo,
                            EventHandler = SaturationSalvoDamage
                        });
                }
            }
        }

        private void SaturationSalvoDamage(object sender, System.EventArgs e)
        {
            var allShips = Roster.AllShips.Select(x => x.Value).ToList();

            foreach (GenericShip ship in allShips)
            {

                if (ship.ShipId == Combat.Defender.ShipId) continue;

                BoardTools.DistanceInfo shotInfo = new BoardTools.DistanceInfo(Combat.Defender, ship);

                if (shotInfo.Range == 1 && ship.Agility < (Combat.ChosenWeapon as GenericUpgrade).Cost)
                {
                    Triggers.RegisterTrigger(new Trigger()
                    {
                        Name = ship.ShipId + ": Saturation Salvo Damage Check",
                        TriggerType = TriggerTypes.OnAbilityDirect,
                        TriggerOwner = ship.Owner.PlayerNo,
                        EventHandler = StartSaturationSalvoCheckSubPhase,
                        Sender = ship
                    });
                }
            }

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, Triggers.FinishTrigger);
        }

        private void StartSaturationSalvoCheckSubPhase(object sender, System.EventArgs e)
        {
            Selection.ActiveShip = sender as GenericShip;
            Phases.StartTemporarySubPhaseOld(
                Selection.ActiveShip.ShipId + ": Saturation Salvo Check",
                typeof(SubPhases.SaturationSalvoCheckSubPhase),
                delegate
                {
                    Phases.FinishSubPhase(typeof(SubPhases.SaturationSalvoCheckSubPhase));
                    Triggers.FinishTrigger();
                }
            );
        }
    }
}

namespace SubPhases
{

    public class SaturationSalvoCheckSubPhase : DiceRollCheckSubPhase
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

            switch (CurrentDiceRoll.DiceList[0].Side)
            {
                case DieSide.Blank:
                    NoDamage();
                    break;
                case DieSide.Focus:
                    NoDamage();
                    break;
                case DieSide.Success:
                    Messages.ShowErrorToHuman("Damage is dealt!");
                    SufferDamage();
                    break;
                case DieSide.Crit:
                    Messages.ShowErrorToHuman("Critical damage is dealt!");
                    SufferDamage();
                    break;
                default:
                    break;
            }
        }

        private void NoDamage()
        {
            Messages.ShowInfoToHuman("No damage");
            CallBack();
        }

        private void SufferDamage()
        {
            foreach (var dice in CurrentDiceRoll.DiceList)
            {
                Selection.ActiveShip.AssignedDamageDiceroll.DiceList.Add(dice);

                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Saturation Salvo Damage",
                    TriggerType = TriggerTypes.OnDamageIsDealt,
                    TriggerOwner = Selection.ActiveShip.Owner.PlayerNo,
                    EventHandler = Selection.ActiveShip.SufferDamage,
                    Skippable = true,
                    EventArgs = new DamageSourceEventArgs()
                    {
                        Source = "Saturation Salvo",
                        DamageType = DamageTypes.CardAbility
                    }
                });
            }

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, CallBack);
        }

    }
}


namespace Abilities.SecondEdition
{
    public class SaturationSalvoAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModificationsOpposite += RegisterSaturationSalvoAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModificationsOpposite -= RegisterSaturationSalvoAbility;
        }

        private void RegisterSaturationSalvoAbility(GenericShip host)
        {
            GenericSecondaryWeapon weapon = Combat.ChosenWeapon as GenericSecondaryWeapon;
            if (weapon != null)
            {
                if (weapon.HasType(UpgradeType.Torpedo) || weapon.HasType(UpgradeType.Missile))
                {
                    ActionsList.GenericAction newAction = new ActionsList.SaturationSalvoActionEffect()
                    {
                        ImageUrl = HostUpgrade.ImageUrl,
                        Host = host
                    };
                    host.AddAvailableDiceModification(newAction);
                }
            }
        }
    }
}

namespace ActionsList
{
    public class SaturationSalvoActionEffect : GenericAction
    {

        public SaturationSalvoActionEffect()
        {
            Name = DiceModificationName = "Saturation Salvo";
            DiceModificationTiming = DiceModificationTimingType.Opposite;
        }

        public override int GetDiceModificationPriority()
        {
            return 80;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;
            GenericSecondaryWeapon weapon = Combat.ChosenWeapon as GenericSecondaryWeapon;
            
            if (Combat.AttackStep == CombatStep.Defence && weapon.UsesCharges && weapon.Charges > 0)
            {
                result = true;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            GenericSecondaryWeapon weapon = Combat.ChosenWeapon as GenericSecondaryWeapon;
            weapon.SpendCharge();
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                NumberOfDiceCanBeRerolled = 2,
                IsOpposite = true,
                CallBack = callBack
            };
            diceRerollManager.Start();
        }

    }
}