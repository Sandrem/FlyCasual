using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameCommands
{
    public class AssignHotacManeuverCommand : GameCommand
    {
        public AssignHotacManeuverCommand(GameCommandTypes type, Type subPhase, string rawParameters) : base(type, subPhase, rawParameters)
        {

        }

        public override void Execute()
        {
            ShipMovementScript.AssignManeuverSimple(
                int.Parse(GetString("id")),
                GetString("maneuver")
            );
        }
    }

}
