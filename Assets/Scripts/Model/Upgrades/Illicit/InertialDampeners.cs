using Upgrade;
using RuleSets;
using Ship;
using Movement;
using SubPhases;
using Tokens;

namespace UpgradesList
{
    public class InertialDampeners : GenericUpgrade, ISecondEditionUpgrade
    {
        public InertialDampeners() : base()
        {
            Types.Add(UpgradeType.Illicit);
            Name = "Inertial Dampeners";
            Cost = 1;

            UpgradeAbilities.Add(new Abilities.InertialDampenersAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 1;

            UpgradeAbilities.RemoveAll(a => a is Abilities.InertialDampenersAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.InertialDampenersAbilitySE());
        }
    }
}

namespace Abilities
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

namespace Abilities
{
    namespace SecondEdition
    {
        public class InertialDampenersAbilitySE : InertialDampenersAbility
        {
            protected override void CheckAbility(GenericShip ship)
            {
                if (HostShip.Shields > 0) RegisterTrigger();
            }

            protected override void FinishAbility()
            {
                HostShip.LoseShield();
                Triggers.FinishTrigger();
            }
        }
    }
}