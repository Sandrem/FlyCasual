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
            Name = "Combat SubPhase";

            if (DebugManager.DebugPhases) Debug.Log("Combat - Started");
        }

        public override void Prepare()
        {
            RequiredPlayer = Phases.PlayerWithInitiative;
            RequiredPilotSkill = PILOTSKILL_MAX + 1;
        }

        public override void Initialize()
        {
            Next();
        }

        public override void Next()
        {
            if (DebugManager.DebugPhases) Debug.Log("Combat SubPhase - Next");

            UI.HideSkipButton();

            if (Selection.ThisShip != null)
            {
                Selection.ThisShip.CallAfterAttackWindow();
            }

            Selection.DeselectAllShips();

            bool success = GetNextActivation(RequiredPilotSkill);
            if (!success)
            {
                int nextPilotSkill = GetNextPilotSkill(RequiredPilotSkill);

                if (nextPilotSkill != RequiredPilotSkill)
                {
                    Phases.CallCombatSubPhaseRequiredPilotSkillIsChanged();
                }

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
                if (DebugManager.DebugPhases) Debug.Log("Attack time for: " + RequiredPlayer + ", skill " + RequiredPilotSkill);

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
            Phases.CurrentSubPhase = new CombatEndSubPhase();
            Phases.CurrentSubPhase.Start();
            Phases.CurrentSubPhase.Prepare();
            Phases.CurrentSubPhase.Initialize();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
        {
            bool result = false;
            if ((ship.Owner.PlayerNo == RequiredPlayer) && (ship.PilotSkill == RequiredPilotSkill) && (Roster.GetPlayer(RequiredPlayer).GetType() == typeof(Players.HumanPlayer)))
            {
                result = true;
            }
            else
            {
                Messages.ShowErrorToHuman("Ship cannot be selected:\n Need " + RequiredPlayer + " and pilot skill " + RequiredPilotSkill);
            }
            return result;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip targetShip)
        {
            bool result = false;
            if (Roster.GetPlayer(RequiredPlayer).GetType() != typeof(Players.NetworkOpponentPlayer))
            {
                if (Selection.ThisShip != null)
                {
                    if (targetShip.Owner.PlayerNo != Phases.CurrentSubPhase.RequiredPlayer)
                    {
                        //TODO: what to show is there are 2 ways (arc and not arc) ?
                        //TODO: clear on skip combat
                        if (Combat.ChosenWeapon == null || Combat.ChosenWeapon.Host.ShipId != Selection.ThisShip.ShipId) Combat.ChosenWeapon = Selection.ThisShip.PrimaryWeapon;
                        Combat.ShotInfo = new Board.ShipShotDistanceInformation(Selection.ThisShip, targetShip, Combat.ChosenWeapon);
                        MovementTemplates.ShowFiringArcRange(Combat.ShotInfo);
                        result = true;
                    }
                    else
                    {
                        Messages.ShowErrorToHuman("Ship cannot be selected as attack target: Friendly ship");
                    }
                }
                else
                {
                    Messages.ShowErrorToHuman("Ship cannot be selected as attack target:\nFirst select attacker");
                }
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
                        GameObject.Find("UI").transform.Find("ContextMenuPanel").Find("FireButton").gameObject.SetActive(true);
                        result++;
                    }
                    else
                    {
                        Messages.ShowErrorToHuman("Your ship already has attacked");
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

        public override void SkipButton()
        {
            foreach (var shipHolder in Roster.GetPlayer(Phases.CurrentPhasePlayer).Ships)
            {
                if (shipHolder.Value.PilotSkill == Phases.CurrentSubPhase.RequiredPilotSkill)
                {
                    shipHolder.Value.IsAttackPerformed = true;
                }
            }
            Next();
        }

    }

}
