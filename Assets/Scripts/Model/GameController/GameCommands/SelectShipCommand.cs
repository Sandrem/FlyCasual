using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameCommands
{
    public class SelectShipCommand : GameCommand
    {
        public SelectShipCommand(GameCommandTypes type, Type subPhase, string rawParameters) : base(type, subPhase, rawParameters)
        {

        }

        public override void Execute()
        {
            SelectShipSubPhase.SelectShip(int.Parse(GetString("id")));
        }
    }

}
