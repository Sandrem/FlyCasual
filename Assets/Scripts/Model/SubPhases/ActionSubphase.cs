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

            if (!Game.Selection.ThisShip.IsBumped)
            {
                //Todo: check if bumped
                if (!Game.Selection.ThisShip.IsDestroyed)
                {
                    Game.Selection.ThisShip.GenerateAvailableActionsList();
                    Game.Roster.GetPlayer(RequiredPlayer).PerformAction();
                }
                else
                {
                    Game.Phases.Next();
                }
                
            }
            else
            {
                Game.Selection.ThisShip.IsBumped = false;
                Game.UI.ShowError("Collision: Skips \"Perform Action\" step");
                Game.UI.AddTestLogEntry("Collision: Skips \"Perform Action\" step");
                NextSubPhase();
            }
            UpdateHelpInfo();
        }

        public override void NextSubPhase()
        {
            Dictionary<int, Players.PlayerNo> pilots = Game.Roster.NextPilotSkillAndPlayerAfter(RequiredPilotSkill, RequiredPlayer, Sorting.Asc);
            foreach (var pilot in pilots)
            {
                Game.Phases.CurrentSubPhase = PreviousSubPhase;
                Game.Phases.CurrentSubPhase.RequiredPilotSkill = pilot.Key;
                Game.Phases.CurrentSubPhase.RequiredPlayer = pilot.Value;
                UpdateHelpInfo();
                Game.Roster.GetPlayer(RequiredPlayer).PerformManeuver();
            }

            if (pilots.Count == 0)
            {
                Game.Phases.CurrentPhase.NextPhase();
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
