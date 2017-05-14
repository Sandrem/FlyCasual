using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Sorting
{
    Asc,
    Desc
}

namespace SubPhases
{

    public class GenericSubPhase
    {
        protected GameManagerScript Game;

        public GenericSubPhase PreviousSubPhase { get; set; }

        public string Name;
        public bool isTemporary = false;

        public int RequiredPilotSkill;
        public Player RequiredPlayer = Player.Player1;

        protected const int PILOTSKILL_MIN = 0;
        protected const int PILOTSKILL_MAX = 12;

        public virtual void StartSubPhase()
        {

        }

        public virtual void NextSubPhase()
        {

        }

        public virtual bool ThisShipCanBeSelected(Ship.GenericShip ship)
        {
            bool result = false;
            if ((ship.PlayerNo == RequiredPlayer) && (ship.PilotSkill == RequiredPilotSkill))
            {
                result = true;
            }
            else
            {
                Game.UI.ShowError("Ship cannot be selected:\n Need " + Game.PhaseManager.CurrentSubPhase.RequiredPlayer + " and pilot skill " + Game.PhaseManager.CurrentSubPhase.RequiredPilotSkill);
            }
            return result;
        }

        public virtual bool AnotherShipCanBeSelected(Ship.GenericShip targetShip)
        {
            bool result = false;
            Game.UI.ShowError("Ship of another player");
            return result;
        }

        public virtual int CountActiveButtons(Ship.GenericShip ship)
        {
            int result = 0;
            return result;
        }

        public virtual void CallNextSubPhase()
        {
            Game.UI.HideTemporaryMenus();
            Game.Ruler.ReturnRangeRuler();
            NextSubPhase();
        }

        public virtual int GetStartingPilotSkill()
        {
            return PILOTSKILL_MIN - 1;
        }

        protected void UpdateHelpInfo()
        {
            Game.UI.Helper.UpdateHelpInfo();
        }

        //TODO: move
        protected Player AnotherPlayer(Player player)
        {
            return (player == Player.Player1) ? Player.Player2 : Player.Player1;
        }

    }

}
