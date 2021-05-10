using System;

namespace GameCommands
{
    public class ActivateAndMoveCommand : GameCommand
    {
        public ActivateAndMoveCommand(GameCommandTypes type, Type subPhase, int subphaseId, string rawParameters) : base(type, subPhase, subphaseId, rawParameters)
        {

        }

        public override void Execute()
        {
            int shipId = int.Parse(GetString("id"));

            Console.Write($"\nMovement activation of : {Roster.GetShipById("ShipId:" + shipId).PilotInfo.PilotName} (ID:{shipId})");

            ShipMovementScript.ActivateAndMove(shipId);
        }
    }

}
