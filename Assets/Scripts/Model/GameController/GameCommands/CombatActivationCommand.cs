using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameCommands
{
    public class CombatActivationCommand : GameCommand
    {
        public CombatActivationCommand(GameCommandTypes type, Type subPhase, string rawParameters) : base(type, subPhase, rawParameters)
        {

        }

        public override void Execute()
        {
            int shipId = int.Parse(GetString("id"));

            Console.Write($"Combat activation of : {Roster.GetShipById("ShipId:" + shipId).PilotInfo.PilotName} (ID:{shipId})");

            CombatSubPhase.DoCombatActivation(shipId);
        }
    }

}
