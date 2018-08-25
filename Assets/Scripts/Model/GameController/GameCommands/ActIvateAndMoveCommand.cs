using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameCommands
{
    public class ActIvateAndMoveCommand : GameCommand
    {
        public ActIvateAndMoveCommand(GameCommandTypes type, Type subPhase, string rawParameters) : base(type, subPhase, rawParameters)
        {

        }

        public override void Execute()
        {
            ShipMovementScript.ActivateAndMove(
                int.Parse(GetString("id"))
            );
        }
    }

}
