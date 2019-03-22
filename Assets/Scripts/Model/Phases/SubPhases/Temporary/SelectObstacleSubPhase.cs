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
using GameCommands;

namespace SubPhases
{
    public class SelectObstacleSubPhase : GenericSubPhase
    {
        public override List<GameCommandTypes> AllowedGameCommandTypes { get { return new List<GameCommandTypes>() { GameCommandTypes.SelectObstacle, GameCommandTypes.PressSkip }; } }

        private static Action<GenericObstacle> SelectTargetAction;
        private Func<GenericObstacle, bool> FilterTargets;

        public string AbilityName;
        public string Description;
        public IImageHolder ImageSource;

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

        public void PrepareByParameters(Action<GenericObstacle> selectTargetAction, Func<GenericObstacle, bool> filterTargets, PlayerNo subphaseOwnerPlayerNo, bool showSkipButton, string abilityName, string description, IImageHolder imageSource = null)
        {
            SelectTargetAction = selectTargetAction;
            FilterTargets = filterTargets;
            RequiredPlayer = subphaseOwnerPlayerNo;
            if (showSkipButton) UI.ShowSkipButton();
            AbilityName = abilityName;
            Description = description;
            ImageSource = imageSource;
        }

        public override void Initialize()
        {
            // If not skipped
            if (Phases.CurrentSubPhase == this)
            {
                IsReadyForCommands = true;
                Roster.GetPlayer(RequiredPlayer).SelectObstacleForAbility();
            }

            // Will be called: HighlightObstacleToSelect();
        }

        public void HighlightObstacleToSelect()
        {
            ShowSubphaseDescription(AbilityName, Description, ImageSource);
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
            Messages.ShowError("Select an obstacle.");
            return false;
        }

        public override bool AnotherShipCanBeSelected(GenericShip targetShip, int mouseKeyIsPressed)
        {
            Messages.ShowError("Select an obstacle.");
            return false;
        }

        public override void ProcessClick()
        {
            if (Roster.GetPlayer(RequiredPlayer).GetType() != typeof(HumanPlayer)) return;

            TryToSelectObstacle();
        }

        private void TryToSelectObstacle()
        {
            if (!EventSystem.current.IsPointerOverGameObject() &&
                (Input.touchCount == 0 || !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)))
            {
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    RaycastHit hitInfo = new RaycastHit();
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
                    {
                        if (hitInfo.transform.tag.StartsWith("Asteroid"))
                        {
                            GenericObstacle clickedObstacle = ObstaclesManager.GetObstacleByTransform(hitInfo.transform);

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
                ConfirmSelectionOfObstacle(obstacle);
            }
            else
            {
                Messages.ShowError("This obstacle cannot be selected.");
            }
        }

        private void ConfirmSelectionOfObstacle(GenericObstacle obstacle)
        {
            GameMode.CurrentGameMode.ExecuteCommand(GenerateSelectObstacleCommand(obstacle.ObstacleGO.name));
        }

        private GameCommand GenerateSelectObstacleCommand(string obstacleName)
        {
            JSONObject parameters = new JSONObject();
            parameters.AddField("name", obstacleName);
            return GameController.GenerateGameCommand(
                GameCommandTypes.SelectObstacle,
                Phases.CurrentSubPhase.GetType(),
                parameters.ToString()
            );
        }

        public static void ConfirmSelectionOfObstacle(string obstacleName)
        {
            GenericObstacle obstacle = ObstaclesManager.GetObstacleByName(obstacleName);
            SelectTargetAction(obstacle);
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
