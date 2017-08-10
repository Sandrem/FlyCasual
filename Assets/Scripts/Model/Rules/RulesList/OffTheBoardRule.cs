using UnityEngine;

namespace RulesList
{
    public class OffTheBoardRule
    {
        private GameManagerScript Game;

        public OffTheBoardRule(GameManagerScript game)
        {
            Game = game;
        }

        public void CheckOffTheBoard(Ship.GenericShip ship)
        {
            foreach (var obj in ship.GetStandEdgePoints())
            {
                if ((Mathf.Abs(obj.Value.x) > Game.PLAYMAT_SIZE/2) || (Mathf.Abs(obj.Value.z) > Game.PLAYMAT_SIZE/2))
                {
                    Messages.ShowError("Ship left the play area and was destroyed!");
                    ship.DestroyShip(true);
                    return;
                }
            }
        }

    }
}
