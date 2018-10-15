using Abilities;
using GameModes;
using Movement;
using RuleSets;
using Ship;
using System;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class SeasonedNavigator : GenericUpgrade, ISecondEditionUpgrade
    {
        public SeasonedNavigator() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Seasoned Navigator";
            Cost = 5;

            UpgradeRuleType = typeof(SecondEdition);

            UpgradeAbilities.Add(new Abilities.SecondEdition.SeasonedNavigator());

            SEImageNumber = 47;
        }

        public void AdaptUpgradeToSecondEdition()
        {
            // Not required
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
                Messages.ShowInfo(HostUpgrade.Name + ": Complexity of maneuver is increased");

                HostShip.AfterGetManeuverColorIncreaseComplexity += ChangeComplexity;
                HostShip.OnMovementFinish += ClearAbility;
            }

            GameMode.CurrentGameMode.AssignManeuver(maneuverCode);
        }

        private bool IsSameSpeedNotRed(string maneuverString)
        {
            bool result = false;

            if (maneuverString == HostShip.AssignedManeuver.ToString()) return true;

            MovementStruct movementStruct = new MovementStruct(maneuverString);
            if (movementStruct.Speed == HostShip.AssignedManeuver.ManeuverSpeed && movementStruct.ColorComplexity != MovementComplexity.Complex)
            {
                result = true;
            }
            return result;
        }

        private void ChangeComplexity(GenericShip ship, ref MovementStruct movement)
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