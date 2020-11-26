using SubPhases;
using System;

namespace GameCommands
{
    public class SystemActivationCommand : GameCommand
    {
        public SystemActivationCommand(GameCommandTypes type, Type subPhase, string rawParameters) : base(type, subPhase, rawParameters)
        {

        }

        public override void Execute()
        {
            int shipId = int.Parse(GetString("id"));

            Console.Write($"System activation of : {Roster.GetShipById("ShipId:" + shipId).PilotInfo.PilotName} (ID:{shipId})");
            SystemsSubPhase.DoSystemActivation(shipId);
        }
    }

}
