using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class SetupSubPhase : GenericSubPhase
    {

        public override void StartSubPhase()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "Setup SubPhase";
            Game.UI.AddTestLogEntry(Name);

            RequiredPilotSkill = GetStartingPilotSkill();

            Phases.CurrentSubPhase.NextSubPhase();
        }

        public override void NextSubPhase()
        {
            Selection.DeselectAllShips();

            Dictionary<int, Players.PlayerNo> pilots = Roster.NextPilotSkillAndPlayerAfter(RequiredPilotSkill, RequiredPlayer, Sorting.Asc);
            foreach (var pilot in pilots)
            {
                RequiredPilotSkill = pilot.Key;
                RequiredPlayer = pilot.Value;
            }

            UpdateHelpInfo();

            if (pilots.Count == 0)
            {
                Board.TurnOffStartingZones();
                Phases.NextPhase();
            } else
            {
                Board.HighlightStartingZones();
                Roster.GetPlayer(RequiredPlayer).SetupShip();
            }

            

        }

    }

}
