using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class PlanningSubPhase : GenericSubPhase
    {

        public override void StartSubPhase()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "Planning SubPhase";

            RequiredPlayer = Game.PhaseManager.PlayerWithInitiative;
            RequiredPilotSkill = GetStartingPilotSkill();

            Game.UI.AddTestLogEntry(Name);

            UpdateHelpInfo();

            Game.Roster.GetPlayer(RequiredPlayer).AssignManeuver();
        }

        public override void NextSubPhase()
        {
            if (Game.Roster.AllManuersAreAssigned(RequiredPlayer))
            {
                if (RequiredPlayer == Game.PhaseManager.PlayerWithInitiative)
                {
                    RequiredPlayer = AnotherPlayer(RequiredPlayer);
                    UpdateHelpInfo();

                    Game.Roster.GetPlayer(RequiredPlayer).AssignManeuver();
                }
                else
                {
                    Game.PhaseManager.CurrentPhase.NextPhase();
                }
            }
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
                Game.UI.ShowError("Ship cannot be selected: Wrong player");
            }
            return result;
        }

        public override int CountActiveButtons(Ship.GenericShip ship)
        {
            int result = 0;
            Game.UI.panelContextMenu.transform.Find("MoveMenuButton").gameObject.SetActive(true);
            result++;
            return result;
        }

        //TODO: Move
        private Players.PlayerNo AnotherPlayer(Players.PlayerNo playerNo)
        {
            return (playerNo == Players.PlayerNo.Player1) ? Players.PlayerNo.Player2 : Players.PlayerNo.Player1;
        }

    }

}
