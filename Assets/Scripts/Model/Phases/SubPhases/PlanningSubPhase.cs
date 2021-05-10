using System.Collections.Generic;
using UnityEngine;
using Ship;
using Remote;
using GameModes;
using GameCommands;

namespace SubPhases
{

    public class PlanningSubPhase : GenericSubPhase
    {
        public override List<GameCommandTypes> AllowedGameCommandTypes { get { return new List<GameCommandTypes>() { GameCommandTypes.PressNext, GameCommandTypes.SelectShipToAssignManeuver, GameCommandTypes.AssignManeuver }; } }

        private static bool IsLocked;
        private int PlayersConfirmedFinish { get; set; }
        private static bool IsPlanningFinished { get; set; }

        public override bool AllowsMultiplayerSelection => Global.IsNetworkGame;

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
            Console.Write($"\nPlanning Phase (Round:{Phases.RoundCounter})", isBold: true, color: "orange");

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

            IsLocked = false;
            IsReadyForCommands = true;
            IsPlanningFinished = false;
            Roster.GetPlayer(RequiredPlayer).AssignManeuversStart();
        }

        public override void Next()
        {
            if (!Global.IsNetworkGame)
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
            else
            {
                PlayersConfirmedFinish++;
                if (PlayersConfirmedFinish == 2) FinishPhase();
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
            if (
                ((ship.Owner.PlayerNo == RequiredPlayer) && (Roster.GetPlayer(RequiredPlayer).GetType() == typeof(Players.HumanPlayer)))
                || (Global.IsNetworkGame == true && ship.Owner is Players.HumanPlayer)
            )
            {
                if (!(ship is GenericRemote))
                {
                    if (IsPlanningFinished)
                    {
                        Messages.ShowError("Your Planning Phase is finished");
                    }
                    else
                    {
                        result = true;
                    }
                }
                else
                {
                    Messages.ShowErrorToHuman("Remotes cannot be assigned a maneuver");
                }
            }
            else
            {
                Messages.ShowErrorToHuman(ship.PilotInfo.PilotName + " cannot be selected: it is owned by the another player");
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
                && (ship.Owner.PlayerNo == RequiredPlayer
                    || (Global.IsNetworkGame == true && ship.Owner is Players.HumanPlayer))
                && !RulesList.IonizationRule.IsIonized(ship)
                && !(ship is GenericRemote);
        }

        public override void NextButtonLocal()
        {
            IsPlanningFinished = true;
        }

        public override void NextButton()
        {
            if (DirectionsMenu.IsVisible) UI.HideDirectionMenu();
            Next();
        }

        public override void DoSelectThisShip(GenericShip ship, int mouseKeyIsPressed)
        {
            if (ship is GenericRemote || IsLocked) return;

            if (!RulesList.IonizationRule.IsIonized(ship))
            {
                IsLocked = true;

                Selection.ChangeActiveShip(ship);
                DirectionsMenu.Show(
                    SendAssignManeuverCommand,
                    CheckForFinish,
                    isRegularPlanning: true
                );
            }
            else
            {
                Messages.ShowError("This ship is ionized. A maneuver cannot be assigned to it");
            }
        }

        private void SendAssignManeuverCommand(string maneuverCode)
        {
            DirectionsMenu.FinishManeuverSelections();

            JSONObject parameters = new JSONObject();
            parameters.AddField("id", Selection.ThisShip.ShipId.ToString());
            parameters.AddField("maneuver", maneuverCode);

            GameMode.CurrentGameMode.ExecuteCommand(
                GameController.GenerateGameCommand
                (
                    GameCommandTypes.AssignManeuver,
                    typeof(PlanningSubPhase),
                    Phases.CurrentSubPhase.ID,
                    parameters.ToString()
                )
            );
        }

        public static void CheckForFinish()
        {
            Roster.HighlightShipOff(Selection.ThisShip);
            IsLocked = false;

            if (!Global.IsNetworkGame)
            {
                if (Roster.AllManuversAreAssigned(Phases.CurrentPhasePlayer))
                {
                    UI.ShowNextButton();
                    UI.HighlightNextButton();
                }
            }
            else
            {
                if (Roster.AllManuversAreAssigned(Global.MyPlayer))
                {
                    if (!IsPlanningFinished)
                    {
                        UI.ShowNextButton();
                        UI.HighlightNextButton();
                    }
                }
            }
        }

        public static GameCommand GenerateSelectShipToAssignManeuver(int shipId)
        {
            JSONObject parameters = new JSONObject();
            parameters.AddField("id", shipId.ToString());
            return GameController.GenerateGameCommand(
                GameCommandTypes.SelectShipToAssignManeuver,
                Phases.CurrentSubPhase.GetType(),
                Phases.CurrentSubPhase.ID,
                parameters.ToString()
            );
        }

        public override void Resume()
        {
            CheckForFinish();
        }
    }

}
