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
                new Vector3(GetFloat("positionX"), GetFloat("positionY"), GetFloat("positionZ")),
                new Vector3(GetFloat("rotationX"), GetFloat("rotationY"), GetFloat("rotationZ"))
            );
        }
    }

}
