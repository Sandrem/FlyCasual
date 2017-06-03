using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class CombatSubPhase : GenericSubPhase
    {

        public override void StartSubPhase()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "Combat SubPhase";

            RequiredPlayer = Phases.PlayerWithInitiative;
            RequiredPilotSkill = GetStartingPilotSkill();

            Game.UI.AddTestLogEntry(Name);

            NextSubPhase();
        }

        public override void NextSubPhase()
        {
            if (Selection.ThisShip != null)
            {
                Selection.ThisShip.CallAfterAttackWindow();
            }

            Selection.DeselectAllShips();

            Dictionary<int, Players.PlayerNo> pilots = Roster.NextPilotSkillAndPlayerAfter(RequiredPilotSkill, RequiredPlayer, Sorting.Desc);
            foreach (var pilot in pilots)
            {
                RequiredPilotSkill = pilot.Key;
                RequiredPlayer = pilot.Value;
            }

            UpdateHelpInfo();

            if (pilots.Count == 0)
            {
                Phases.CurrentPhase.NextPhase();
            }
            else
            {
                Roster.HighlightShipsFiltered(RequiredPlayer, RequiredPilotSkill);
                Game.UI.ShowSkipButton();
                Roster.GetPlayer(RequiredPlayer).PerformAttack();
            }
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

        public override bool AnotherShipCanBeSelected(Ship.GenericShip targetShip)
        {
            bool result = false;
            if (Selection.ThisShip != null)
            {
                if (targetShip.Owner.PlayerNo != Phases.CurrentSubPhase.RequiredPlayer)
                {
                    result = true;
                }
                else
                {
                    Game.UI.ShowError("Ship cannot be selected as attack target: Friendly ship");
                }
            }
            else
            {
                Game.UI.ShowError("Ship cannot be selected as attack target:\nFirst select attacker");
            }
            return result;
        }

        public override int CountActiveButtons(Ship.GenericShip ship)
        {
            int result = 0;
            if (Selection.ThisShip != null)
            {
                if (ship.Owner.PlayerNo != Phases.CurrentSubPhase.RequiredPlayer)
                {
                    if (Selection.ThisShip.IsAttackPerformed != true)
                    {
                        Game.PrefabsList.ContextMenuPanel.transform.Find("FireButton").gameObject.SetActive(true);
                        result++;
                    }
                    else
                    {
                        Game.UI.ShowError("Your ship already has attacked");
                    }
                }
            }
            return result;
        }

        public override int GetStartingPilotSkill()
        {
            return PILOTSKILL_MAX + 1;
        }

    }

}
