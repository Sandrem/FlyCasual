
using UnityEngine;

namespace RulesList
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
            Phases.OnEndPhaseStart += EndPhaseClearAll;
        }

        public void EndPhaseClearAll()
        {
            ClearShipTokens();
        }

        private void ClearShipTokens()
        {
            foreach (var shipHolder in Roster.AllShips)
            {
                shipHolder.Value.ClearAllTokens();
                shipHolder.Value.ClearAlreadyExecutedActions();
            }
        }
    }
}
