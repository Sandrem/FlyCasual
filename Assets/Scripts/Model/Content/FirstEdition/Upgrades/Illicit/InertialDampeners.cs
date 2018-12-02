using Movement;
using Ship;
using SubPhases;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class InertialDampeners : GenericUpgrade
    {
        public InertialDampeners() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Inertial Dampeners",
                UpgradeType.Illicit,
                cost: 1,
                abilityType: typeof(Abilities.FirstEdition.InertialDampenersAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
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

        protected virtual void CheckAbility(GenericShip ship)
        {
            RegisterTrigger();
        }

        protected void RegisterTrigger()
        {
            RegisterAbilityTrigger(TriggerTypes.OnManeuverIsReadyToBeRevealed, AskToUseInertialDampenersAbility);
        }

        private void AskToUseInertialDampenersAbility(object sender, System.EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, UseInertialDampenersAbility);
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

        protected virtual void FinishAbility()
        {
            HostUpgrade.TryDiscard(Triggers.FinishTrigger);
        }
    }
}