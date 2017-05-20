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
            Game.UI.ActionsPanel.ShowActionsPanel();
        }

        public override void PerformFreeAction()
        {
            Game.UI.ActionsPanel.ShowFreeActionsPanel();
        }

        public override void UseDiceModifications()
        {
            Combat.ShowDiceResultMenu();
        }

    }

}
