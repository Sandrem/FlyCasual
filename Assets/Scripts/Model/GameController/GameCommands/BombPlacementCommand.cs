using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameCommands
{
    public class BombPlacementCommand : GameCommand
    {
        public BombPlacementCommand(GameCommandTypes type, Type subPhase, string rawParameters) : base(type, subPhase, rawParameters)
        {

        }

        public override void Execute()
        {
            PlaceBombTokenSubphase.FinishBombPlacement(
                 new Vector3(GetFloat("positionX"), GetFloat("positionY"), GetFloat("positionZ")),
                 new Vector3(GetFloat("rotationX"), GetFloat("rotationY"), GetFloat("rotationZ"))
            );
        }
    }

}
