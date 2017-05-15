
using UnityEngine;

namespace Rules
{
    public class EndPhaseRule
    {
        private GameManagerScript Game;

        public EndPhaseRule(GameManagerScript game)
        {
            Game = game;
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            Game.PhaseManager.OnEndPhaseStart += EndPhaseClearAll;
        }

        public void EndPhaseClearAll()
        {
            ClearShipTokens();
        }

        private void ClearShipTokens()
        {
            foreach (var shipHolder in Game.Roster.AllShips)
            {
                shipHolder.Value.ClearAllTokens();
                shipHolder.Value.ClearAlreadyExecutedActions();
            }
        }
    }
}
