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
        public Players.PlayerNo RequiredPlayer = Players.PlayerNo.Player1;

        protected const int PILOTSKILL_MIN = 0;
        protected const int PILOTSKILL_MAX = 12;

        public virtual void Start()
        {
            
        }

        public virtual void Prepare()
        {

        }

        public virtual void Initialize()
        {

        }

        public virtual void Next()
        {

        }

        public virtual void FinishPhase()
        {

        }

        public virtual bool ThisShipCanBeSelected(Ship.GenericShip ship)
        {
            bool result = false;
            if ((ship.Owner.PlayerNo == RequiredPlayer) && (ship.PilotSkill == RequiredPilotSkill))
            {
                result = true;
            }
            else
            {
                Game.UI.ShowError("Ship cannot be selected:\n Need " + Phases.CurrentSubPhase.RequiredPlayer + " and pilot skill " + Phases.CurrentSubPhase.RequiredPilotSkill);
            }
            return result;
        }

        public virtual bool AnotherShipCanBeSelected(Ship.GenericShip targetShip)
        {
            bool result = false;
            Game.UI.ShowError("Ship of another player");
            return result;
        }

        //TODO: What is this?
        public virtual int CountActiveButtons(Ship.GenericShip ship)
        {
            int result = 0;
            return result;
        }

        //TODO: What is this?
        public virtual void CallNextSubPhase()
        {
            Game.UI.HideTemporaryMenus();
            MovementTemplates.ReturnRangeRuler();
            Next();
        }

        //TODO: What is this?
        public virtual int GetStartingPilotSkill()
        {
            return PILOTSKILL_MIN - 1;
        }

        protected void UpdateHelpInfo()
        {
            Phases.UpdateHelpInfo();
        }

        public virtual void DoDefault()
        {

        }

    }

}
