
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
            foreach (var shipHolder in Roster.AllShips)
            {
                ClearShipTokens(shipHolder.Value);
                ClearShipFlags(shipHolder.Value);
            }
        }

        private void ClearShipTokens(Ship.GenericShip ship)
        {
            ship.ClearAllTokens();
            ship.ClearAlreadyExecutedActions();
        }

        private void ClearShipFlags(Ship.GenericShip ship)
        {
            ship.IsAttackPerformed = false;
            ship.IsManeuverPerformed = false;
            ship.IsSkipsActionSubPhase = false;
        }
    }
}
