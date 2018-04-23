using Ship;
using Upgrade;
using System.Linq;
using Abilities;
using Tokens;
using SquadBuilderNS;
using SubPhases;

namespace UpgradesList
{
    public class PulsedRayShield : GenericUpgrade
    {
        public PulsedRayShield() : base()
        {
            Types.Add(UpgradeType.Modification);
            Name = "Pulsed Ray Shield";
            Cost = 2;

            UpgradeAbilities.Add(new PulsedRayShieldAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return (ship.faction == Faction.Rebel || ship.faction == Faction.Scum) && ((ship.MaxShields == 1));
        }

        public override bool IsAllowedForSquadBuilderPostCheck(SquadList squadList)
        {
            bool result = Host.MaxShields == 1;

            if (!result) Messages.ShowError("Pulsed Ray Shield cannot be installed on ships where shield value is not 1!");

            return result;
        }
    }
}

namespace Abilities
{
    public class PulsedRayShieldAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.OnEndPhaseStart_Triggers += RegisterPulsedRayShieldAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.OnEndPhaseStart_Triggers -= RegisterPulsedRayShieldAbility;
        }

        private void RegisterPulsedRayShieldAbility()
        {
            if (!HostShip.IsDestroyed && HostShip.Shields < HostShip.MaxShields)
            {
                RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, AskPulsedRayShield);
            }
        }

        private void AskPulsedRayShield(object sender, System.EventArgs e)
        {
            if (HostShip.Shields < HostShip.MaxShields)
            {
                //Selection.ChangeActiveShip(HostShip);
                AskToUseAbility(ShouldUseAbility, PulsedRayShieldConfirm);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool ShouldUseAbility()
        {
            bool result = false;
            if (IsAlreadyIonized() || HasTurret()) result = true;
            return result;
        }

        private bool IsAlreadyIonized()
        {
            return HostShip.Tokens.HasToken(typeof(IonToken));
        }

        private bool HasTurret()
        {
            bool result = false;

            if (HostShip.PrimaryWeapon.CanShootOutsideArc)
            {
                result = true;
            }
            else
            {
                foreach(GenericUpgrade upgrade in HostShip.UpgradeBar.GetUpgradesOnlyFaceup())
                {
                    IShipWeapon weapon = upgrade as IShipWeapon;
                    if (weapon != null)
                    {
                        if (weapon.CanShootOutsideArc)
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }

            return result;
        }

        private void PulsedRayShieldConfirm(object sender, System.EventArgs e)
        {
            HostShip.Tokens.AssignToken(new IonToken(HostShip), RestoreShield);
        }

        private void RestoreShield()
        {
            if (HostShip.TryRegenShields()) Messages.ShowInfo("Pulsed Ray Shield: Shield is restored");
            DecisionSubPhase.ConfirmDecision();
        }
    }
}
