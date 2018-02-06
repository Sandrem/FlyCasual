
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using UnityEngine;

namespace RulesList
{
    public class EndPhaseCleanupRule
    {

        public EndPhaseCleanupRule()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            Phases.OnRoundStart += InitializeAll;
            Phases.OnRoundEnd += RegisterClearAll;
        }

        private void InitializeAll()
        {
            foreach (var ship in Roster.AllShips.Values)
            {
                ship.IsCannotAttackSecondTime = false;
            }
        }

        private void RegisterClearAll()
        {
            Triggers.RegisterTrigger(new Trigger
            {
                Name = "End of the round: Clear all",
                TriggerOwner = Players.PlayerNo.Player1,
                TriggerType = TriggerTypes.OnRoundEnd,
                EventHandler = EndPhaseClearAll
            });
        }

        public void EndPhaseClearAll(object sender, System.EventArgs e)
        {
            List<GenericToken> tokensList = new List<GenericToken>();

            foreach (var shipHolder in Roster.AllShips)
            {
                ClearShipFlags(shipHolder.Value);
                ClearAssignedManeuvers(shipHolder.Value);
                shipHolder.Value.ClearAlreadyExecutedActions();

                List<GenericToken> allShipTokens = shipHolder.Value.Tokens.GetAllTokens();
                if (allShipTokens != null) tokensList.AddRange(allShipTokens.Where(n => n.Host.ShouldRemoveTokenInEndPhase(n)));
            }

            ClearShipTokens(tokensList, Triggers.FinishTrigger);
        }

        private void ClearShipTokens(List<GenericToken> tokensList, Action callback)
        {
            Actions.RemoveTokens(tokensList, callback);
        }

        private void ClearShipFlags(Ship.GenericShip ship)
        {
            ship.IsAttackPerformed = false;
            ship.IsManeuverPerformed = false;
            ship.IsSkipsActionSubPhase = false;
            ship.IsBombAlreadyDropped = false;
            ship.IsCannotAttackSecondTime = false;
        }

        private void ClearAssignedManeuvers(Ship.GenericShip ship)
        {
            ship.ClearAssignedManeuver();
        }
    }
}
