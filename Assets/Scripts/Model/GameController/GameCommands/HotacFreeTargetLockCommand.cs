using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameCommands
{
    public class HotacFreeTargetLockCommand : GameCommand
    {
        public HotacFreeTargetLockCommand(GameCommandTypes type, Type subPhase, string rawParameters) : base(type, subPhase, rawParameters)
        {

        }

        public override void Execute()
        {
            Selection.ChangeActiveShip("ShipId:" + int.Parse(GetString("id")));

            Actions.AcquireTargetLock(
                Roster.GetShipById("ShipId:" + int.Parse(GetString("id"))),
                Roster.GetShipById("ShipId:" + int.Parse(GetString("target"))),
                delegate {},
                delegate {}
            );
        }
    }

}
