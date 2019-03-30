using SubPhases;
using System;
using System.Collections.Generic;
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
                 new Vector3(float.Parse(GetString("positionX")), float.Parse(GetString("positionY")), float.Parse(GetString("positionZ"))),
                 new Vector3(float.Parse(GetString("rotationX")), float.Parse(GetString("rotationY")), float.Parse(GetString("rotationZ")))
            );
        }
    }

}
