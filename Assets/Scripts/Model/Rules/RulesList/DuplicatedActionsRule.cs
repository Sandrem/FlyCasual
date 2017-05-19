
using UnityEngine;

namespace RulesList
{
    public class DuplicatedActionsRule
    {
        private GameManagerScript Game;

        public DuplicatedActionsRule(GameManagerScript game)
        {
            Game = game;
        }

        public void CanPerformActions(ActionsList.GenericAction action, ref bool result)
        {
            if (Game.Selection.ThisShip.AlreadyExecutedAction(action.GetType())) result = false;
        }

    }
}
