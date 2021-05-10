using SubPhases;
using System;

namespace GameCommands
{
    public class SystemActivationCommand : GameCommand
    {
        public SystemActivationCommand(GameCommandTypes type, Type subPhase, int subphaseId, string rawParameters) : base(type, subPhase, subphaseId, rawParameters)
        {

        }

        public override void Execute()
        {
            int shipId = int.Parse(GetString("id"));

            Console.Write($"\nSystem activation of : {Roster.GetShipById("ShipId:" + shipId).PilotInfo.PilotName} (ID:{shipId})");
            SystemsSubPhase.DoSystemActivation(shipId);
        }
    }

}
