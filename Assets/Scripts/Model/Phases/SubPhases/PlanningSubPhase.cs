using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;

namespace SubPhases
{

    public class PlanningSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            base.Start();
            Name = "Planning SubPhase";
        }

        public override void Prepare()
        {
            RequiredPlayer = Phases.PlayerWithInitiative;
            RequiredPilotSkill = PILOTSKILL_MIN - 1;
        }

        public override void Initialize()
        {
            UpdateHelpInfo();
            Roster.HighlightShipsFiltered(FilterShipsToAssignManeuver);
            Roster.GetPlayer(RequiredPlayer).AssignManeuver();
        }

        public override void Next()
        {
            if (Roster.AllManuversAreAssigned(RequiredPlayer))
            {
                HideAssignedManeuversInHotSeatGame();

                if (RequiredPlayer == Phases.PlayerWithInitiative)
                {
                    RequiredPlayer = Roster.AnotherPlayer(RequiredPlayer);

                    UpdateHelpInfo();
                    Roster.HighlightShipsFiltered(FilterShipsToAssignManeuver);
                    Roster.GetPlayer(RequiredPlayer).AssignManeuver();
                }
                else
                {
                    FinishPhase();
                }
            }
        }

        private void HideAssignedManeuversInHotSeatGame()
        {
            if (Roster.GetPlayer(Roster.AnotherPlayer(RequiredPlayer)).GetType() != typeof(Players.HotacAiPlayer))
            {
                foreach (var shipHolder in Roster.GetPlayer(RequiredPlayer).Ships)
                {
                    Roster.ToggelManeuverVisibility(shipHolder.Value, false);
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
                result = true;
            }
            else
            {
                Messages.ShowErrorToHuman("Ship cannot be selected: Wrong player");
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
            return ship.AssignedManeuver == null && ship.Owner.PlayerNo == RequiredPlayer;
        }

        public override void NextButton()
        {
            if (DirectionsMenu.IsVisible) UI.HideDirectionMenu();
            Next();
        }

        public override void DoSelectThisShip(GenericShip ship, int mouseKeyIsPressed)
        {
            UI.ShowDirectionMenu();
        }

    }

}
