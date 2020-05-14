using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModes;
using Tokens;

namespace ActionsList
{

    public class SlamAction : GenericAction
    {
        private bool canBePerformedAsFreeAction = false;

        public SlamAction()
        {
            Name = DiceModificationName = "SLAM";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/SlamAction.png";
        }

        public SlamAction(bool canBePerformedAsFreeAction) : this()
        {
            this.canBePerformedAsFreeAction = canBePerformedAsFreeAction;
        }


        public override bool CanBePerformedAsAFreeAction
        {
            get
            {
                return canBePerformedAsFreeAction;
            }
        }

        public override void ActionTake()
        {
            if (Selection.ThisShip.Owner.UsesHotacAiRules)
            {
                Phases.CurrentSubPhase.CallBack();
            }
            else
            {
                Phases.CurrentSubPhase.Pause();

                Selection.ThisShip.Owner.SelectManeuver(ShipMovementScript.SendAssignManeuverCommand, ExecuteSelectedManeuver, IsSameSpeed);
            }
        }

        private void ExecuteSelectedManeuver()
        {
            Selection.ThisShip.AssignedManeuver.IsRevealDial = false;
            ShipMovementScript.LaunchMovement(AssignWeaponsDisabledToken);
        }

        private void AssignWeaponsDisabledToken()
        {
            Selection.ThisShip.Tokens.AssignToken(typeof(WeaponsDisabledToken), FinishSlam);
        }

        private void FinishSlam()
        {
            Selection.ThisShip.CallSlam(Phases.CurrentSubPhase.CallBack);
        }

        private void PerformSlamManeuver(object sender, System.EventArgs e)
        {
            Selection.ThisShip.AssignedManeuver.Perform();
        }

        private bool IsSameSpeed(string maneuverString)
        {
            bool result = false;
            Movement.ManeuverHolder movementStruct = new Movement.ManeuverHolder(maneuverString);
            if (movementStruct.Speed == Selection.ThisShip.AssignedManeuver.ManeuverSpeed)
            {
                result = true;
            }
            return result;
        }

        public override int GetActionPriority()
        {
            int result = 0;
            return result;
        }

    }

}
