using System;

namespace GameCommands
{
    public class ActivateAndMoveCommand : GameCommand
    {
        public ActivateAndMoveCommand(GameCommandTypes type, Type subPhase, string rawParameters) : base(type, subPhase, rawParameters)
        {

        }

        public override void Execute()
        {
            int shipId = int.Parse(GetString("id"));

            Console.Write($"Movement activation of : {Roster.GetShipById("ShipId:" + shipId).PilotInfo.PilotName} (ID:{shipId})");

            ShipMovementScript.ActivateAndMove(shipId);
        }
    }

}
