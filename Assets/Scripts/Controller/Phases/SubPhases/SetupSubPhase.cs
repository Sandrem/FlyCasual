using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class SetupSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            Debug.Log("Start");
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "Setup SubPhase";
            Game.UI.AddTestLogEntry(Name);
        }

        public override void Initialize()
        {
            Debug.Log("Init");
            RequiredPilotSkill = PILOTSKILL_MIN - 1;
            Next();
        }

        public override void Next()
        {
            Debug.Log("Next");
            //CHANGE
            Dictionary<int, Players.PlayerNo> pilots = Roster.NextPilotSkillAndPlayerAfter(RequiredPilotSkill, RequiredPlayer, Sorting.Asc);
            foreach (var pilot in pilots)
            {
                RequiredPilotSkill = pilot.Key;
                RequiredPlayer = pilot.Value;
            }

            UpdateHelpInfo();

            if (pilots.Count == 0)
            {
                Finish();
            } else
            {
                //Board.HighlightStartingZones();
                Roster.HighlightShipsFiltered(RequiredPlayer, RequiredPilotSkill);
                Roster.GetPlayer(RequiredPlayer).SetupShip();
            }

        }

        public override void Finish()
        {
            Debug.Log("Finish");
            Board.TurnOffStartingZones();
            Phases.NextPhase();
        }

    }

}
