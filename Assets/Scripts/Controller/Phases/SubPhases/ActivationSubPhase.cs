using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class ActivationSubPhase : GenericSubPhase
    {

        public override void StartSubPhase()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "Activation SubPhase";

            Game.UI.AddTestLogEntry(Name);

            Dictionary<int, Players.PlayerNo> pilots = Roster.NextPilotSkillAndPlayerAfter(RequiredPilotSkill, RequiredPlayer, Sorting.Asc);
            foreach (var pilot in pilots)
            {
                RequiredPilotSkill = pilot.Key;
                RequiredPlayer = pilot.Value;
                UpdateHelpInfo();
            }

            Roster.HighlightShipsFiltered(RequiredPlayer, RequiredPilotSkill);
            Roster.GetPlayer(RequiredPlayer).PerformManeuver();
        }

        public override void NextSubPhase()
        {
            Phases.CurrentSubPhase = new ActionSubPhase();
            Phases.CurrentSubPhase.PreviousSubPhase = this;
            Phases.CurrentSubPhase.RequiredPilotSkill = RequiredPilotSkill;
            Phases.CurrentSubPhase.RequiredPlayer = RequiredPlayer;
            Phases.CurrentSubPhase.StartSubPhase();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
        {
            bool result = false;

            if ((ship.Owner.PlayerNo == RequiredPlayer) && (ship.PilotSkill == RequiredPilotSkill))
            {
                result = true;
            }
            else
            {
                Game.UI.ShowError("Ship cannot be selected:\n Need " + RequiredPlayer + " and pilot skill " + RequiredPilotSkill);
            }
            return result;
        }

        public override int CountActiveButtons(Ship.GenericShip ship)
        {
            int result = 0;
            if (Selection.ThisShip.AssignedManeuver != null)
            {
                Game.PrefabsList.ContextMenuPanel.transform.Find("MovePerformButton").gameObject.SetActive(true);
                result++;
            }
            else
            {
                Game.UI.ShowError("This ship has already executed his maneuver");
            };
            return result;
        }

    }

}
