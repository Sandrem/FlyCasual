using UnityEngine;

namespace RulesList
{
    public class OffTheBoardRule
    {
        public void CheckOffTheBoard(Ship.GenericShip ship)
        {
            foreach (var obj in ship.GetStandEdgePoints())
            {
                if ((Mathf.Abs(obj.Value.x) > Board.BoardManager.PLAYMAT_SIZE/2) || (Mathf.Abs(obj.Value.z) > Board.BoardManager.PLAYMAT_SIZE/2))
                {
                    Messages.ShowError("Ship left the play area and was destroyed!");
                    ship.DestroyShip(delegate { }, true);
                    return;
                }
            }
        }

    }
}
