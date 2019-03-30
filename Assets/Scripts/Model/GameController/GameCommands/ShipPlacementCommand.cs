using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameCommands
{
    public class ShipPlacementCommand : GameCommand
    {
        public ShipPlacementCommand(GameCommandTypes type, Type subPhase, string rawParameters) : base(type, subPhase, rawParameters)
        {

        }

        public override void Execute()
        {
            SetupSubPhase.PlaceShip(
                int.Parse(GetString("id")),
                new Vector3(float.Parse(GetString("positionX")), float.Parse(GetString("positionY")), float.Parse(GetString("positionZ"))),
                new Vector3(float.Parse(GetString("rotationX")), float.Parse(GetString("rotationY")), float.Parse(GetString("rotationZ")))
            );
        }
    }

}
