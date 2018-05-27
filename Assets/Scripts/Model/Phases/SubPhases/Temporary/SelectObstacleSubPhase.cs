using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModes;
using Ship;
using System;
using System.Linq;
using Players;
using UnityEngine.UI;

namespace SubPhases
{
    public class SelectObstacleSubPhase : GenericSubPhase
    {
        private Action finishAction;

        public string AbilityName;
        public string Description;
        public string ImageUrl;

        public override void Start()
        {
            IsTemporary = true;

            Prepare();
            Initialize();

            UpdateHelpInfo();

            base.Start();
        }

        public override void Prepare()
        {

        }

        public void PrepareByParameters(Action selectTargetAction, PlayerNo subphaseOwnerPlayerNo, bool showSkipButton, string abilityName, string description, string imageUrl = null)
        {
            finishAction = selectTargetAction;
            RequiredPlayer = subphaseOwnerPlayerNo;
            if (showSkipButton) UI.ShowSkipButton();
            AbilityName = abilityName;
            Description = description;
            ImageUrl = imageUrl;
        }

        public override void Initialize()
        {
            // If not skipped
            //if (Phases.CurrentSubPhase == this) Roster.GetPlayer(RequiredPlayer).SelectObstacleForAbility();
            HighlightObstacleToSelect();
        }

        public void HighlightObstacleToSelect()
        {
            ShowSubphaseDescription(AbilityName, Description, ImageUrl);
            //Highlight filtered obstacles
        }

        public override void Next()
        {
            UI.HideSkipButton();

            //Dehighigh obstacles
            HideSubphaseDescription();

            Phases.CurrentSubPhase = Phases.CurrentSubPhase.PreviousSubPhase;
            UpdateHelpInfo();

            CallBack();
        }

        public override void SkipButton()
        {
            Next();
        }

        public override bool ThisShipCanBeSelected(GenericShip ship, int mouseKeyIsPressed)
        {
            Messages.ShowError("Select an obstacle");
            return false;
        }

        public override bool AnotherShipCanBeSelected(GenericShip targetShip, int mouseKeyIsPressed)
        {
            Messages.ShowError("Select an obstacle");
            return false;
        }
    }

}
