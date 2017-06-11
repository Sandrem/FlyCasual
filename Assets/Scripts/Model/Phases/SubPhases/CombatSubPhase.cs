using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SubPhases
{

    public class CombatSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "Combat SubPhase";
        }

        public override void Initialize()
        {
            RequiredPlayer = Phases.PlayerWithInitiative;
            RequiredPilotSkill = PILOTSKILL_MAX + 1;
            Next();
        }

        public override void Next()
        {
            if (Selection.ThisShip != null)
            {
                Selection.ThisShip.CallAfterAttackWindow();
            }

            Selection.DeselectAllShips();

            bool success = GetNextActivation(RequiredPilotSkill);
            if (!success)
            {
                int nextPilotSkill = GetNextPilotSkill(RequiredPilotSkill);
                if (nextPilotSkill != int.MaxValue)
                {
                    success = GetNextActivation(nextPilotSkill);
                }
                else
                {
                    FinishPhase();
                }
            }

            if (success)
            {
                UpdateHelpInfo();
                HighlightShips();
                Roster.GetPlayer(RequiredPlayer).PerformAttack();
            }
        }

        private bool GetNextActivation(int pilotSkill)
        {

            bool result = false;

            var pilotSkillResults =
                from n in Roster.AllShips
                where n.Value.PilotSkill == pilotSkill
                where n.Value.IsAttackPerformed == false
                select n;

            if (pilotSkillResults.Count() > 0)
            {
                RequiredPilotSkill = pilotSkill;

                var playerNoResults =
                    from n in pilotSkillResults
                    where n.Value.Owner.PlayerNo == Phases.PlayerWithInitiative
                    select n;

                if (playerNoResults.Count() > 0)
                {
                    RequiredPlayer = Phases.PlayerWithInitiative;
                }
                else
                {
                    RequiredPlayer = Roster.AnotherPlayer(Phases.PlayerWithInitiative);
                }

                result = true;
            }

            return result;
        }

        private int GetNextPilotSkill(int pilotSkillMax)
        {
            int result = int.MaxValue;

            var ascPilotSkills =
                from n in Roster.AllShips
                where n.Value.PilotSkill < pilotSkillMax
                orderby n.Value.PilotSkill
                select n;

            if (ascPilotSkills.Count() > 0)
            {
                result = ascPilotSkills.Last().Value.PilotSkill;
            }

            return result;
        }

        public override void FinishPhase()
        {
            Phases.CurrentPhase.NextPhase();
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
                    MovementTemplates.ShowFiringArcRange(Selection.ThisShip, targetShip);
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

        private void HighlightShips()
        {
            Roster.AllShipsHighlightOff();
            foreach (var ship in Roster.GetPlayer(RequiredPlayer).Ships)
            {
                if ((ship.Value.PilotSkill == RequiredPilotSkill) && (!ship.Value.IsAttackPerformed))
                {
                    ship.Value.HighlightCanBeSelectedOn();
                    Roster.RosterPanelHighlightOn(ship.Value);
                }
            }
        }

    }

}
