using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Players
{

    public partial class HumanPlayer : GenericPlayer
    {

        public HumanPlayer() : base()
        {
            Type = PlayerType.Human;
            Name = "Human";
        }

        public override void PerformAction()
        {

            List<ActionsList.GenericAction> availableActions = Selection.ThisShip.GetAvailableActionsList();
            foreach (var action in availableActions)
            {
                (Phases.CurrentSubPhase as SubPhases.DecisionSubPhase).AddDecision(action.Name, delegate {
                    Tooltips.EndTooltip();
                    Game.UI.HideNextButton();
                    Selection.ThisShip.AddAlreadyExecutedAction(action);
                    action.ActionTake();
                });
            }
        }

        public override void PerformFreeAction()
        {
            Actions.ShowFreeActionsPanel();
        }

        public override void PerformAttack()
        {
            Game.UI.ShowSkipButton();
        }

        public override void UseDiceModifications()
        {
            Combat.ShowDiceResultMenu(Combat.ConfirmDiceResults);
        }

        public override void TakeDecision()
        {
            Game.PrefabsList.PanelDecisions.SetActive(true);
        }

    }

}
