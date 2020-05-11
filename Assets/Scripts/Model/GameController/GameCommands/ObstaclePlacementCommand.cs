using Players;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace GameCommands
{
    public class ObstaclePlacementCommand : GameCommand
    {
        public ObstaclePlacementCommand(GameCommandTypes type, Type subPhase, string rawParameters) : base(type, subPhase, rawParameters)
        {

        }

        public override void Execute()
        {
            ObstaclesPlacementSubPhase.PlaceObstacle
            (
                GetString("name"),
                new Vector3
                (
                    float.Parse(GetString("positionX"), CultureInfo.InvariantCulture),
                    0,
                    float.Parse(GetString("positionZ"), CultureInfo.InvariantCulture)
                ),
                new Vector3
                (
                    float.Parse(GetString("rotationX"), CultureInfo.InvariantCulture),
                    float.Parse(GetString("rotationY"), CultureInfo.InvariantCulture),
                    float.Parse(GetString("rotationZ"), CultureInfo.InvariantCulture)
                )
             );
        }
    }

}
