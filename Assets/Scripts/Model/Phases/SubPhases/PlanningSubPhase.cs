using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class PlanningSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
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
            HighlightShips();
            Roster.GetPlayer(RequiredPlayer).AssignManeuver();
        }

        public override void Next()
        {
            if (Roster.AllManuersAreAssigned(RequiredPlayer))
            {
                if (RequiredPlayer == Phases.PlayerWithInitiative)
                {
                    RequiredPlayer = Roster.AnotherPlayer(RequiredPlayer);

                    UpdateHelpInfo();
                    HighlightShips();
                    Roster.GetPlayer(RequiredPlayer).AssignManeuver();
                }
                else
                {
                    FinishPhase();
                }
            }
        }

        public override void FinishPhase()
        {
            Phases.CurrentPhase.NextPhase();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
        {
            bool result = false;
            if (ship.Owner.PlayerNo == RequiredPlayer)
            {
                result = true;
            }
            else
            {
                Messages.ShowErrorToHuman("Ship cannot be selected: Wrong player");
            }
            return result;
        }

        public override int CountActiveButtons(Ship.GenericShip ship)
        {
            int result = 0;
            Game.PrefabsList.ContextMenuPanel.transform.Find("MoveMenuButton").gameObject.SetActive(true);
            result++;
            return result;
        }

        private void HighlightShips()
        {
            Roster.AllShipsHighlightOff();
            foreach (var ship in Roster.GetPlayer(RequiredPlayer).Ships)
            {
                ship.Value.HighlightCanBeSelectedOn();
                Roster.RosterPanelHighlightOn(ship.Value);
            }
        }

    }

}
