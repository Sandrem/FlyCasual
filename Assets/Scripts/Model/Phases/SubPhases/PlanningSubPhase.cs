using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using Remote;
using System;
using GameModes;
using GameCommands;

namespace SubPhases
{

    public class PlanningSubPhase : GenericSubPhase
    {
        public override List<GameCommandTypes> AllowedGameCommandTypes { get { return new List<GameCommandTypes>() { GameCommandTypes.PressNext, GameCommandTypes.SelectShipToAssignManeuver }; } }

        public override void Start()
        {
            base.Start();
            Name = "Planning SubPhase";
        }

        public override void Prepare()
        {
            RequiredPlayer = Phases.PlayerWithInitiative;
            RequiredInitiative = PILOTSKILL_MIN - 1;
        }

        public override void Initialize()
        {
            Roster.AllShipsHighlightOff();
            PlayerAssignsManeuvers();
        }

        private void PlayerAssignsManeuvers()
        {
            UpdateHelpInfo();
            if (!(Roster.GetPlayer(RequiredPlayer) is Players.GenericAiPlayer)) Roster.HighlightShipsFiltered(FilterShipsToAssignManeuver);

            if (Roster.AllManuversAreAssigned(Phases.CurrentPhasePlayer))
            {
                UI.ShowNextButton();
                UI.HighlightNextButton();
            }

            IsReadyForCommands = true;
            Roster.GetPlayer(RequiredPlayer).AssignManeuversStart();
        }

        public override void Next()
        {
            if (Roster.AllManuversAreAssigned(RequiredPlayer))
            {
                HideAssignedManeuversInHotSeatGame();

                if (RequiredPlayer == Phases.PlayerWithInitiative)
                {
                    RequiredPlayer = Roster.AnotherPlayer(RequiredPlayer);
                    PlayerAssignsManeuvers();
                }
                else
                {
                    FinishPhase();
                }
            }
        }

        private void HideAssignedManeuversInHotSeatGame()
        {
            if (Roster.GetPlayer(Roster.AnotherPlayer(RequiredPlayer)).UsesHotacAiRules == false)
            {
                foreach (var shipHolder in Roster.GetPlayer(RequiredPlayer).Ships)
                {
                    Roster.ToggleManeuverVisibility(shipHolder.Value, false);
                }
            }
        }

        public override void FinishPhase()
        {
            Phases.CurrentPhase.NextPhase();
        }

        public override bool ThisShipCanBeSelected(GenericShip ship, int mouseKeyIsPressed)
        {
            bool result = false;
            if ((ship.Owner.PlayerNo == RequiredPlayer) && (Roster.GetPlayer(RequiredPlayer).GetType() == typeof(Players.HumanPlayer)))
            {
                if (!(ship is GenericRemote))
                {
                    result = true;
                }
                else
                {
                    Messages.ShowErrorToHuman("Remotes cannot be assigned a maneuver");
                }
            }
            else
            {
                Messages.ShowErrorToHuman(ship.PilotInfo.PilotName + " cannot be selected: it is owned by the wrong player");
            }
            return result;
        }

        public override int CountActiveButtons(GenericShip ship)
        {
            int result = 0;
            GameObject.Find("UI").transform.Find("ContextMenuPanel").Find("MoveMenuButton").gameObject.SetActive(true);
            result++;
            return result;
        }

        private bool FilterShipsToAssignManeuver(GenericShip ship)
        {
            return ship.AssignedManeuver == null
                && ship.Owner.PlayerNo == RequiredPlayer
                && !RulesList.IonizationRule.IsIonized(ship)
                && !(ship is GenericRemote);
        }

        public override void NextButton()
        {
            if (DirectionsMenu.IsVisible) UI.HideDirectionMenu();
            Next();
        }

        public override void DoSelectThisShip(GenericShip ship, int mouseKeyIsPressed)
        {
            if (ship is GenericRemote) return;

            if (!RulesList.IonizationRule.IsIonized(ship))
            {
                GameCommand command = GenerateSelectShipToAssignManeuver(ship.ShipId);
                GameMode.CurrentGameMode.ExecuteCommand(command);
            }
            else
            {
                Messages.ShowError("This ship is ionized. A maneuver cannot be assigned to it");
            }
            
        }

        public static void CheckForFinish()
        {
            Roster.HighlightShipOff(Selection.ThisShip);

            if (Roster.AllManuversAreAssigned(Phases.CurrentPhasePlayer))
            {
                UI.ShowNextButton();
                UI.HighlightNextButton();
            }
        }

        public static GameCommand GenerateSelectShipToAssignManeuver(int shipId)
        {
            JSONObject parameters = new JSONObject();
            parameters.AddField("id", shipId.ToString());
            return GameController.GenerateGameCommand(
                GameCommandTypes.SelectShipToAssignManeuver,
                Phases.CurrentSubPhase.GetType(),
                parameters.ToString()
            );
        }

        public override void Resume()
        {
            CheckForFinish();
        }
    }

}
