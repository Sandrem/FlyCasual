using UnityEngine;
using Ship;

namespace RulesList
{
    public class OffTheBoardRule
    {
        public void CheckOffTheBoard(GenericShip ship)
        {
            foreach (var obj in ship.ShipBase.GetStandEdgePoints())
            {
                if ((Mathf.Abs(obj.Value.x) > Board.BoardManager.PLAYMAT_SIZE/2) || (Mathf.Abs(obj.Value.z) > Board.BoardManager.PLAYMAT_SIZE/2))
                {
                    Triggers.RegisterTrigger(new Trigger()
                    {
                        Name = "Ship is off the board",
                        TriggerType = TriggerTypes.OnPositionFinish,
                        TriggerOwner = ship.Owner.PlayerNo,
                        EventHandler = DestroyShipOffTheBoard,
                        Sender = ship
                    });

                    return;
                }
            }
        }

        private void DestroyShipOffTheBoard(object sender, System.EventArgs e)
        {
            GenericShip ship = sender as GenericShip;

            Messages.ShowError("Ship left the play area and was destroyed!");
            ship.DestroyShipForced(Triggers.FinishTrigger);
        }

    }
}
