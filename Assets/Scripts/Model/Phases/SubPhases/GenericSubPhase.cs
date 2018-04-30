using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Ship;
using Players;

public enum Sorting
{
    Asc,
    Desc
}

namespace SubPhases
{

    public class GenericSubPhase
    {
        public GenericSubPhase PreviousSubPhase { get; set; }

        public string Name;

        public Action CallBack;

        public bool IsTemporary = false;

        private bool canBePaused;
        public bool CanBePaused
        {
            get { return canBePaused; }
            set { canBePaused = value; }
        }

        private GenericShip theShip;
        public GenericShip TheShip
        {
            get { return theShip ?? Selection.ThisShip; }
            set { theShip = value; }
        }

        public int RequiredPilotSkill;
        public PlayerNo RequiredPlayer = PlayerNo.Player1;

        protected const int PILOTSKILL_MIN = 0;
        protected const int PILOTSKILL_MAX = 12;

        public virtual void Start()
        {
            Roster.HighlightPlayer(RequiredPlayer);
        }

        public virtual void Prepare() { }

        public virtual void Initialize() { }

        public virtual void Pause() { }

        public virtual void Resume()
        {
            Roster.HighlightPlayer(RequiredPlayer);
        }

        public virtual void Update() { }

        public virtual void ProcessClick() { }

        public virtual void Next() { }

        public virtual void FinishPhase() { }

        public virtual bool ThisShipCanBeSelected(GenericShip ship, int mouseKeyIsPressed)
        {
            bool result = false;
            if ((ship.Owner.PlayerNo == RequiredPlayer) && (ship.PilotSkill == RequiredPilotSkill) && (Roster.GetPlayer(RequiredPlayer).GetType() == typeof(Players.HumanPlayer)))
            {
                result = true;
            }
            else
            {
                Messages.ShowErrorToHuman("Ship cannot be selected:\n Need " + Phases.CurrentSubPhase.RequiredPlayer + " and pilot skill " + Phases.CurrentSubPhase.RequiredPilotSkill);
            }
            return result;
        }

        public virtual bool AnotherShipCanBeSelected(GenericShip targetShip, int mouseKeyIsPressed)
        {
            bool result = false;
            Messages.ShowErrorToHuman("Ship of another player");
            return result;
        }

        //TODO: What is this?
        public virtual int CountActiveButtons(GenericShip ship)
        {
            int result = 0;
            return result;
        }

        //TODO: What is this?
        public virtual void CallNextSubPhase()
        {
            UI.HideTemporaryMenus();
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

        public virtual void DoDefault() { }

        public virtual void NextButton() { }

        public virtual void SkipButton() { }

        public virtual void DoSelectThisShip(GenericShip ship, int mouseKeyIsPressed) { }

        public virtual void DoSelectAnotherShip(GenericShip ship, int mouseKeyIsPressed) { }

    }

}
