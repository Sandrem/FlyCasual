using GameModes;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameCommands
{
    public class SelectShipToAssignManeuverCommand : GameCommand
    {
        public SelectShipToAssignManeuverCommand(GameCommandTypes type, Type subPhase, string rawParameters) : base(type, subPhase, rawParameters)
        {

        }

        public override void Execute()
        {
            Selection.ChangeActiveShip("ShipId:" + int.Parse(GetString("id")));
            DirectionsMenu.Show(ShipMovementScript.SendAssignManeuverCommand, PlanningSubPhase.CheckForFinish, isRegularPlanning: true);
        }
    }

}
