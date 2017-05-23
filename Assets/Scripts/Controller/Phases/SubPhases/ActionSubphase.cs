using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class ActionSubPhase : GenericSubPhase
    {

        public override void StartSubPhase()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "Action SubPhase";
            Game.UI.AddTestLogEntry(Name);

            if (!Selection.ThisShip.IsBumped)
            {
                //Todo: check if bumped
                if (!Selection.ThisShip.IsDestroyed)
                {
                    Selection.ThisShip.GenerateAvailableActionsList();
                    Roster.GetPlayer(RequiredPlayer).PerformAction();
                }
                else
                {
                    Phases.Next();
                }
                
            }
            else
            {
                Selection.ThisShip.IsBumped = false;
                Game.UI.ShowError("Collision: Skips \"Perform Action\" step");
                Game.UI.AddTestLogEntry("Collision: Skips \"Perform Action\" step");
                NextSubPhase();
            }
            UpdateHelpInfo();
        }

        public override void NextSubPhase()
        {
            Dictionary<int, Players.PlayerNo> pilots = Roster.NextPilotSkillAndPlayerAfter(RequiredPilotSkill, RequiredPlayer, Sorting.Asc);
            foreach (var pilot in pilots)
            {
                Phases.CurrentSubPhase = PreviousSubPhase;
                Phases.CurrentSubPhase.RequiredPilotSkill = pilot.Key;
                Phases.CurrentSubPhase.RequiredPlayer = pilot.Value;

                UpdateHelpInfo();
                Roster.HighlightShips(Phases.CurrentSubPhase.RequiredPlayer, Phases.CurrentSubPhase.RequiredPilotSkill);
                Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).PerformManeuver();
            }

            if (pilots.Count == 0)
            {
                Phases.CurrentPhase.NextPhase();
            }
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
        {
            bool result = false;
            Game.UI.ShowError("Ship cannot be selected: Perform action first");
            return result;
        }

    }

}
