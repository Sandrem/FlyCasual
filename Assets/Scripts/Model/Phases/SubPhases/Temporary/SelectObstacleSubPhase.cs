using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModes;
using Ship;
using System;
using System.Linq;
using Players;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Obstacles;

namespace SubPhases
{
    public class SelectObstacleSubPhase : GenericSubPhase
    {
        private Action<GenericObstacle> SelectTargetAction;
        private Func<GenericObstacle, bool> FilterTargets;

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

        public void PrepareByParameters(Action<GenericObstacle> selectTargetAction, Func<GenericObstacle, bool> filterTargets, PlayerNo subphaseOwnerPlayerNo, bool showSkipButton, string abilityName, string description, string imageUrl = null)
        {
            SelectTargetAction = selectTargetAction;
            FilterTargets = filterTargets;
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
            //TODO: Highlight filtered obstacles
        }

        public override void Next()
        {
            UI.HideSkipButton();

            //TODO: Turn off highlight of filteredobstacles
            HideSubphaseDescription();

            Phases.CurrentSubPhase = Phases.CurrentSubPhase.PreviousSubPhase;
            UpdateHelpInfo();
        }

        public override void SkipButton()
        {
            SelectObstacle();
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

        public override void ProcessClick()
        {
            if (Roster.GetPlayer(RequiredPlayer).GetType() != typeof(HumanPlayer)) return;

            TryToSelectObstacle();
        }

        private void TryToSelectObstacle()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    RaycastHit hitInfo = new RaycastHit();
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
                    {
                        if (hitInfo.transform.tag.StartsWith("Asteroid"))
                        {
                            GameObject obstacleGO = hitInfo.transform.parent.gameObject;
                            GenericObstacle clickedObstacle = ObstaclesManager.Instance.GetObstacleByName(obstacleGO.name);

                            if (clickedObstacle.IsPlaced)
                            {
                                SelectObstacle(clickedObstacle);
                            }
                        }
                    }
                }
            }
        }

        private void SelectObstacle(GenericObstacle obstacle)
        {
            if (FilterTargets(obstacle))
            {
                SelectTargetAction(obstacle);
            }
            else
            {
                Messages.ShowError("This obstacle cannot be selected");
            }
        }

        public static void SelectObstacle()
        {
            Action callback = Phases.CurrentSubPhase.CallBack;
            Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
            Phases.CurrentSubPhase.Resume();
            callback();
        }
    }

}
