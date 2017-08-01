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

        public override void PerformAction(object sender, EventArgs e)
        {
            Phases.StartTemporarySubPhase("Action", typeof(SubPhases.ActionDecisonSubPhase), delegate () { Phases.FinishSubPhase(typeof(SubPhases.ActionDecisonSubPhase)); Triggers.FinishTrigger(); });
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
