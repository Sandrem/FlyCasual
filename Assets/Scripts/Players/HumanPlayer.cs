using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Players
{

    public class HumanPlayer : GenericPlayer
    {
        public readonly new PlayerType Type;
        public readonly new string Name;
        public readonly new int Id;

        public HumanPlayer(int id) : base(id)
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Type = PlayerType.Human;
            Name = "Human";
            Id = id;
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
