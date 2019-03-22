using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Ship;
using Players;
using UnityEngine.UI;

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

        public bool IsTemporary;

        public bool IsReadyForCommands { get; set; }

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

        public virtual List<GameCommandTypes> AllowedGameCommandTypes { get { return new List<GameCommandTypes>(); } }

        public virtual bool ThisShipCanBeSelected(GenericShip ship, int mouseKeyIsPressed)
        {
            bool result = false;
            if ((ship.Owner.PlayerNo == RequiredPlayer) && (ship.State.Initiative == RequiredPilotSkill) && (Roster.GetPlayer(RequiredPlayer).GetType() == typeof(Players.HumanPlayer)))
            {
                result = true;
            }
            else
            {
                Messages.ShowErrorToHuman(ship.PilotName + " cannot be selected.\n The ship must be owned by " + Phases.CurrentSubPhase.RequiredPlayer + " and have a pilot skill of " + Phases.CurrentSubPhase.RequiredPilotSkill);
            }
            return result;
        }

        public virtual bool AnotherShipCanBeSelected(GenericShip targetShip, int mouseKeyIsPressed)
        {
            bool result = false;
            Messages.ShowErrorToHuman(targetShip.PilotName + "is owned by another player!");
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

        public static void HideSubphaseDescription()
        {
            GameObject subphaseDescriptionGO = GameObject.Find("UI").transform.Find("CurrentSubphaseDescription").gameObject;
            subphaseDescriptionGO.SetActive(false);

            subphaseDescriptionGO = GameObject.Find("UI").transform.Find("CurrentSubphaseDescriptionNoImage").gameObject;
            subphaseDescriptionGO.SetActive(false);
        }

        public void ShowSubphaseDescription(string title, string description, IImageHolder imageSource = null)
        {
            HideSubphaseDescription();
            if (Roster.GetPlayer(RequiredPlayer).GetType() == typeof(HumanPlayer) && title != null)
            {
                GameObject subphaseDescriptionGO = GameObject.Find("UI").transform.Find("CurrentSubphaseDescription" + ((imageSource != null) ? "" : "NoImage")).gameObject; 
                 
                subphaseDescriptionGO.transform.Find("AbilityName").GetComponent<Text>().text = title;
                subphaseDescriptionGO.transform.Find("DescriptionHolder/Description").GetComponent<Text>().text = description;
                if (imageSource != null) subphaseDescriptionGO.transform.Find("DescriptionHolder/CardImage").GetComponent<SmallCardArt>().Initialize(imageSource);

                subphaseDescriptionGO.SetActive(true);
            }
        }

    }

}
