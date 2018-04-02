using UnityEngine;
using Ship;
using Board;

namespace RulesList
{
    public class OffTheBoardRule
    {
        public void CheckOffTheBoard(GenericShip ship)
        {
            if (BoardManager.IsOffTheBoard(ship))
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
        }

        private void DestroyShipOffTheBoard(object sender, System.EventArgs e)
        {
            GenericShip ship = sender as GenericShip;

            Messages.ShowError("Ship left the play area and was destroyed!");
            ship.DestroyShipForced(Triggers.FinishTrigger, true);
        }

    }
}
