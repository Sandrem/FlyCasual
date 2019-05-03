using UnityEngine;
using Ship;
using BoardTools;

public enum Direction
{
    Top,
    Bottom,
    Left,
    Right,
    None
}

namespace RulesList
{
    public class OffTheBoardRule
    {
        public void CheckOffTheBoard(GenericShip ship)
        {
            if (Board.IsOffTheBoard(ship))
            {
                Direction direction = Board.GetOffTheBoardDirection(ship);

                bool shouldDestroyShip = true;
                ship.CallOffTheBoard(ref shouldDestroyShip, direction);
                if (shouldDestroyShip)
                {
                    Triggers.RegisterTrigger(new Trigger()
                    {
                        Name = "Ship is off the board",
                        TriggerType = TriggerTypes.OnPositionFinish,
                        TriggerOwner = ship.Owner.PlayerNo,
                        EventHandler = DestroyShipOffTheBoard,
                        Sender = ship
                    });
                }
                else
                {
                    ship.IsSkipsActionSubPhase = true;
                }
            }
        }

        private void DestroyShipOffTheBoard(object sender, System.EventArgs e)
        {
            GenericShip ship = sender as GenericShip;

            Messages.ShowInfo(ship.PilotInfo.PilotName + " left the play area and was destroyed!");
            ship.DestroyShipForced(Triggers.FinishTrigger, true);
        }

    }
}
