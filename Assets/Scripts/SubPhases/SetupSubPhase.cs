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

            Game.PhaseManager.CurrentSubPhase.NextSubPhase();
        }

        public override void NextSubPhase()
        {
            Game.Selection.DeselectAllShips();

            Dictionary<int, Player> pilots = Game.Roster.NextPilotSkillAndPlayerAfter(RequiredPilotSkill, RequiredPlayer, Sorting.Asc);
            foreach (var pilot in pilots)
            {
                RequiredPilotSkill = pilot.Key;
                RequiredPlayer = pilot.Value;
            }

            UpdateHelpInfo();

            if (RequiredPilotSkill == -1)
            {
                Game.PhaseManager.CurrentPhase.NextPhase();
            }

            Game.Position.HighlightStartingZones();
        }

    }

}
