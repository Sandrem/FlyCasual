using SubPhases;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            int shipId = int.Parse(GetString("id"));

            Console.Write($"Ship is placed on the board: {Roster.GetShipById("ShipId:" + shipId).PilotInfo.PilotName} (ID:{shipId})");

            SetupSubPhase.PlaceShip
            (
                shipId,
                new Vector3
                (
                    float.Parse(GetString("positionX"), CultureInfo.InvariantCulture),
                    float.Parse(GetString("positionY"), CultureInfo.InvariantCulture),
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
