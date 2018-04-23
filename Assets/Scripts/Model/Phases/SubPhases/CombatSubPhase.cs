using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Ship;
using System;
using GameModes;

namespace SubPhases
{

    public class CombatSubPhase : GenericSubPhase
    {
        private Team.Type selectionMode;

        public override void Start()
        {
            base.Start();

            Name = "Combat SubPhase";

            selectionMode = Team.Type.Friendly;

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
                Roster.HighlightShipsFiltered(FilterShipsToAssignManeuver);
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
            if (Phases.HasOnCombatPhaseEndEvents)
            {
                GenericSubPhase subphase = Phases.StartTemporarySubPhaseNew("Notification", typeof(NotificationSubPhase), StartCombatEndSubPhase);
                (subphase as NotificationSubPhase).TextToShow = "End of combat";
                subphase.Start();
            }
            else
            {
                StartCombatEndSubPhase();
            }
        }

        private void StartCombatEndSubPhase()
        {
            Phases.CurrentSubPhase = new CombatEndSubPhase();
            Phases.CurrentSubPhase.Start();
            Phases.CurrentSubPhase.Prepare();
            Phases.CurrentSubPhase.Initialize();
        }

        public override bool ThisShipCanBeSelected(GenericShip ship, int mouseKeyIsPressed)
        {
            bool result = false;

            if ((ship.Owner.PlayerNo == RequiredPlayer) && (ship.PilotSkill == RequiredPilotSkill) && (Roster.GetPlayer(RequiredPlayer).GetType() == typeof(Players.HumanPlayer)))
            {
                if (ship.IsAttackPerformed)
                {
                    Messages.ShowErrorToHuman("Ship cannot be selected:\nShip already performed attack");
                    return result;
                }

                if (selectionMode == Team.Type.Any)
                {
                    Messages.ShowErrorToHuman("Ship cannot be selected:\nUI is locked");
                    return result;
                }

                if (selectionMode == Team.Type.Enemy)
                {
                    Messages.ShowErrorToHuman("Ship cannot be selected:\nAttack by activated ship or skip attack");
                    return result;
                }

                result = true;
            }
            else
            {
                Messages.ShowErrorToHuman("Ship cannot be selected:\nNeed " + RequiredPlayer + " and pilot skill " + RequiredPilotSkill);
            }

            return result;
        }

        public override void DoSelectThisShip(GenericShip ship, int mouseKeyIsPressed)
        {
            Roster.HighlightShipsFiltered(FilterShipsToAttack);

            GameMode.CurrentGameMode.CombatActivation(ship.ShipId);
        }

        private bool FilterShipsToAttack(GenericShip ship)
        {
            return ship.Owner.PlayerNo != RequiredPlayer;
        }

        private void LockSelectionMode()
        {
            UI.HideSkipButton();
            selectionMode = Team.Type.Any;
        }

        public void ChangeSelectionMode(Team.Type type)
        {
            UI.ShowSkipButton();
            selectionMode = type;
        }

        public override bool AnotherShipCanBeSelected(GenericShip targetShip, int mouseKeyIsPressed)
        {
            bool result = false;
            if (Roster.GetPlayer(RequiredPlayer).GetType() != typeof(Players.NetworkOpponentPlayer))
            {
                if (Selection.ThisShip != null)
                {
                    if (targetShip.Owner.PlayerNo != Phases.CurrentSubPhase.RequiredPlayer)
                    {
                        Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(Selection.ThisShip, targetShip);
                        MovementTemplates.ShowFiringArcRange(shotInfo);
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

        public override int CountActiveButtons(GenericShip ship)
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

        private bool FilterShipsToAssignManeuver(GenericShip ship)
        {
            return ship.PilotSkill == RequiredPilotSkill && !ship.IsAttackPerformed && ship.Owner.PlayerNo == RequiredPlayer;
        }

        public override void SkipButton()
        {
            if ((Selection.ThisShip == null) || (Selection.ThisShip.IsAttackPerformed))
            {
                // If no ship is selected or selected ship already performed attack
                // skip combat for all player's ships with current PS

                List<GenericShip> shipsToSkipCombat = new List<GenericShip>();

                foreach (var shipHolder in Roster.GetPlayer(Phases.CurrentPhasePlayer).Ships)
                {
                    if (shipHolder.Value.PilotSkill == Phases.CurrentSubPhase.RequiredPilotSkill)
                    {
                        shipsToSkipCombat.Add(shipHolder.Value);
                    }
                }

                SkipCombatByShips(shipsToSkipCombat, Next);
            }
            else
            {
                // If selected ship can attack - skip attack only for this ship

                AfterSkippedCombatActivation(Selection.ThisShip);
                CheckNext();
            }

        }

        private void SkipCombatByShips(List<GenericShip> shipsToSkipCombat, Action callback)
        {
            if (shipsToSkipCombat != null && shipsToSkipCombat.Count > 0)
            {
                GenericShip shipToSkipCombat = shipsToSkipCombat.First();

                shipsToSkipCombat.Remove(shipToSkipCombat);

                if (!shipToSkipCombat.IsAttackPerformed)
                {
                    shipToSkipCombat.CallCombatActivation(
                    delegate {
                        AfterSkippedCombatActivation(shipToSkipCombat);
                        shipToSkipCombat.CallCombatDeactivation(
                            delegate { SkipCombatByShips(shipsToSkipCombat, callback); }
                        );
                    });
                }
                else
                {
                    SkipCombatByShips(shipsToSkipCombat, callback);
                }
            }
            else
            {
                callback();
            }
        }

        private void AfterSkippedCombatActivation(GenericShip ship)
        {
            if (!ship.IsAttackPerformed) ship.CallAfterAttackWindow();
            ship.IsAttackPerformed = true;

            Selection.DeselectThisShip();
            Selection.DeselectAnotherShip();
            ChangeSelectionMode(Team.Type.Friendly);
        }

        private void CheckNext()
        {
            if (Roster.GetPlayer(RequiredPlayer).Ships.Count(n => n.Value.PilotSkill == RequiredPilotSkill && !n.Value.IsAttackPerformed) == 0)
            {
                Next();
            }
            else
            {
                // TODO: Highlight available ships again
            }
        }

        public override void DoSelectAnotherShip(GenericShip ship, int mouseKeyIsPressed)
        {
            if (mouseKeyIsPressed == 1)
            {
                if (Selection.ThisShip.IsAttackPerformed != true)
                {
                    UI.ClickDeclareTarget();
                }
                else
                {
                    Messages.ShowErrorToHuman("Your ship already has attacked");
                }
            }
            else if (mouseKeyIsPressed == 2)
            {
                UI.CheckFiringRangeAndShow();
            }
        }

        public override void Resume()
        {
            base.Resume();

            ChangeSelectionMode(Team.Type.Friendly);
        }

    }

}
