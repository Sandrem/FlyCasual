using Ship;
using Upgrade;
using System.Collections.Generic;
using GameModes;
using Movement;

namespace UpgradesList.SecondEdition
{
    public class SeasonedNavigator : GenericUpgrade
    {
        public SeasonedNavigator() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Seasoned Navigator",
                UpgradeType.Crew,
                cost: 5,
                abilityType: typeof(Abilities.SecondEdition.SeasonedNavigator),
                seImageNumber: 47
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class SeasonedNavigator : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += ActivateSeasonedNavigator;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= ActivateSeasonedNavigator;
        }

        private void ActivateSeasonedNavigator(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskToChangeManeuver);
        }

        private void AskToChangeManeuver(object sender, System.EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, ShowAvailableManeuver);
        }

        private void ShowAvailableManeuver(object sender, System.EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.Owner.ChangeManeuver(AssignManeuverWithIncreasedComplexity, IsSameSpeedNotRed);
        }

        private void AssignManeuverWithIncreasedComplexity(string maneuverCode)
        {
            if (maneuverCode != HostShip.AssignedManeuver.ToString())
            {
                Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": Complexity of maneuver is increased");

                HostShip.AfterGetManeuverColorIncreaseComplexity += ChangeComplexity;
                HostShip.OnMovementFinish += ClearAbility;
            }

            GameMode.CurrentGameMode.AssignManeuver(maneuverCode);
        }

        private bool IsSameSpeedNotRed(string maneuverString)
        {
            bool result = false;

            if (maneuverString == HostShip.AssignedManeuver.ToString()) return true;

            ManeuverHolder movementStruct = new ManeuverHolder(maneuverString);
            if (movementStruct.Speed == HostShip.AssignedManeuver.ManeuverSpeed && movementStruct.ColorComplexity != MovementComplexity.Complex)
            {
                result = true;
            }
            return result;
        }

        private void ChangeComplexity(GenericShip ship, ref ManeuverHolder movement)
        {
            if (movement.ToString() == HostShip.AssignedManeuver.ToString()) return;

            movement.ColorComplexity = GenericMovement.IncreaseComplexity(movement.ColorComplexity);
        }

        private void ClearAbility(GenericShip ship)
        {
            HostShip.AfterGetManeuverColorIncreaseComplexity -= ChangeComplexity;
            HostShip.OnMovementFinish -= ClearAbility;
        }
    }
}