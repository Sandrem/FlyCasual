using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Players
{

    public partial class HumanPlayer : GenericPlayer
    {

        public HumanPlayer(int id) : base(id)
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Type = PlayerType.Human;
            Name = "Human";
            Id = id;
            PlayerNo = (id == 1) ? PlayerNo.Player1 : PlayerNo.Player2;
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
            Game.UI.DiceResults.panelDiceResultsMenu.SetActive(true);
        }

    }

}
