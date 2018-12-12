using Upgrade;
using Ship;
using System.Collections.Generic;
using System.Linq;
using SquadBuilderNS;
using Tokens;
using SubPhases;

namespace UpgradesList.FirstEdition
{
    public class PulsedRayShield : GenericUpgrade
    {
        public PulsedRayShield() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Pulsed Ray Shield",
                UpgradeType.Modification,
                cost: 2,
                abilityType: typeof(Abilities.FirstEdition.PulsedRayShieldAbility)
            );
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return (ship.Faction == Faction.Rebel || ship.Faction == Faction.Scum) && ((ship.ShipInfo.Shields == 1));
        }

        public override bool IsAllowedForSquadBuilderPostCheck(SquadList squadList)
        {
            bool result = HostShip.ShipInfo.Shields == 1;

            if (!result) Messages.ShowError("Pulsed Ray Shield cannot be installed on ships where shield value is not 1!");

            return result;
        }
    }
}

namespace Abilities.FirstEdition
{
    public class PulsedRayShieldAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers += RegisterPulsedRayShieldAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers -= RegisterPulsedRayShieldAbility;
        }

        private void RegisterPulsedRayShieldAbility()
        {
            if (!HostShip.IsDestroyed && HostShip.State.ShieldsCurrent < HostShip.State.ShieldsMax)
            {
                RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, AskPulsedRayShield);
            }
        }

        private void AskPulsedRayShield(object sender, System.EventArgs e)
        {
            if (HostShip.State.ShieldsCurrent < HostShip.State.ShieldsMax)
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
            // TODOREVERT

            bool result = false;

            /*if (HostShip.PrimaryWeapon.CanShootOutsideArc)
            {
                result = true;
            }
            else
            {*/
                foreach (GenericUpgrade upgrade in HostShip.UpgradeBar.GetUpgradesOnlyFaceup())
                {
                    IShipWeapon weapon = upgrade as IShipWeapon;
                    if (weapon != null)
                    {
                        /*if (weapon.CanShootOutsideArc)
                        {
                            result = true;
                            break;
                        }*/
                    }
                }
            //}

            return result;
        }

        private void PulsedRayShieldConfirm(object sender, System.EventArgs e)
        {
            HostShip.Tokens.AssignToken(typeof(IonToken), RestoreShield);
        }

        private void RestoreShield()
        {
            if (HostShip.TryRegenShields()) Messages.ShowInfo("Pulsed Ray Shield: Shield is restored");
            DecisionSubPhase.ConfirmDecision();
        }
    }
}
