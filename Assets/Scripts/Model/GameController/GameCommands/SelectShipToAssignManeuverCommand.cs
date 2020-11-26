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
            int shipId = int.Parse(GetString("id"));

            Console.Write($"Ship is selected to assign a maneuver: {Roster.GetShipById("ShipId:" + shipId).PilotInfo.PilotName} (ID:{shipId})");

            Selection.ChangeActiveShip("ShipId:" + shipId);
            DirectionsMenu.Show(ShipMovementScript.SendAssignManeuverCommand, PlanningSubPhase.CheckForFinish, isRegularPlanning: true);
        }
    }

}
