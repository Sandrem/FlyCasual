
using UnityEngine;

namespace RulesList
{
    public class EndPhaseCleanupRule
    {

        public EndPhaseCleanupRule(GameManagerScript game)
        {
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
