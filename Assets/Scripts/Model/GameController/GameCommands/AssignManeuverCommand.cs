using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameCommands
{
    public class AssignManeuverCommand : GameCommand
    {
        public AssignManeuverCommand(GameCommandTypes type, Type subPhase, string rawParameters) : base(type, subPhase, rawParameters)
        {

        }

        public override void Execute()
        {
            int shipId = int.Parse(GetString("id"));
            string maneuverCode = GetString("maneuver");

            Console.Write($"Maneuver is assigned: \"{maneuverCode}\" for {Roster.GetShipById("ShipId:" + shipId).PilotInfo.PilotName} (ID:{shipId})");

            ShipMovementScript.AssignManeuver(shipId, maneuverCode);

            if (Phases.CurrentSubPhase is PlanningSubPhase)  PlanningSubPhase.CheckForFinish();
        }
    }

}
