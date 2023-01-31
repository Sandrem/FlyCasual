using Content;
using Movement;
using Ship;
using System.Collections.Generic;
using Tokens;
using Upgrade;
using SubPhases;

namespace UpgradesList.SecondEdition
{
    public class InertialDampeners : GenericUpgrade
    {
        public InertialDampeners() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Inertial Dampeners",
                UpgradeType.Illicit,
                cost: 8,
                abilityType: typeof(Abilities.SecondEdition.InertialDampenersAbility),
                seImageNumber: 61,
                legalityInfo: new List<Legality>
                {
                    Legality.StandardBanned,
                    Legality.ExtendedLegal
                }
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class InertialDampenersAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsReadyToBeRevealed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsReadyToBeRevealed -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (HostShip.State.ShieldsCurrent > 0) RegisterTrigger();
        }

        private void RegisterTrigger()
        {
            RegisterAbilityTrigger(TriggerTypes.OnManeuverIsReadyToBeRevealed, AskToUseInertialDampenersAbility);
        }

        private void AskToUseInertialDampenersAbility(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                UseInertialDampenersAbility,
                descriptionLong: "Do you want to spend 1 shield? (If you do, execute a white stationaty maneuver instead of the maneuver you revealed, then gain 1 stress token)",
                imageHolder: HostUpgrade
            );
        }

        private void UseInertialDampenersAbility(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            GenericMovement movement = new StationaryMovement(0, ManeuverDirection.Stationary, ManeuverBearing.Stationary, MovementComplexity.Normal);
            HostShip.SetAssignedManeuver(movement);
            HostShip.OnMovementFinish += RegisterAssignStressAfterManeuver;

            FinishAbility();
        }

        private void RegisterAssignStressAfterManeuver(GenericShip ship)
        {
            HostShip.OnMovementFinish -= RegisterAssignStressAfterManeuver;

            RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AssignStress);
        }

        private void AssignStress(object sender, System.EventArgs e)
        {
            HostShip.Tokens.AssignToken(typeof(StressToken), Triggers.FinishTrigger);
        }

        private void FinishAbility()
        {
            HostShip.LoseShield();
            Triggers.FinishTrigger();
        }
    }
}