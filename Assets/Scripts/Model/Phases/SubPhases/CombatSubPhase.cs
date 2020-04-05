using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Ship;
using System;
using GameModes;
using BoardTools;
using GameCommands;
using Remote;

namespace SubPhases
{

    public class CombatSubPhase : GenericSubPhase
    {
        public override List<GameCommandTypes> AllowedGameCommandTypes { get { return new List<GameCommandTypes>() { GameCommandTypes.CombatActivation, GameCommandTypes.PressSkip }; } }

        public override void Start()
        {
            base.Start();

            Name = "Combat SubPhase";

            if (DebugManager.DebugPhases) Debug.Log("Combat - Started");
        }

        public override void Prepare()
        {
            RequiredPlayer = Phases.PlayerWithInitiative;
            RequiredInitiative = PILOTSKILL_MAX + 1;
        }

        public override void Initialize()
        {
            Next();
        }

        public override void Next()
        {
            if (DebugManager.DebugPhases) Debug.Log("Combat SubPhase - Next");

            UI.HideSkipButton();
            MovementTemplates.ReturnRangeRuler();
            Selection.DeselectAllShips();

            // Try to get any pilot with same inititative, that didn't perform attack
            bool success = GetNextActivation(RequiredInitiative);

            if (success)
            {
                ReadyForCombatActivation();
            }
            else
            {
                ChangeInitiative();
            }
        }

        private void ChangeInitiative()
        {
            //Just decrement to pass through all possible Initiative values (SE Han Solo Rebel Gunner, for example)
            //RequiredInitiative = GetNextPilotSkill(RequiredInitiative);
            RequiredInitiative--;

            if (RequiredInitiative != -1)
            {
                Phases.Events.CallEngagementInitiativeChanged(Next);
            }
            else
            {
                Phases.Events.CallEngagementInitiativeChanged(FinishPhase);
            }
        }

        private void ReadyForCombatActivation()
        {
            if (DebugManager.DebugPhases) Debug.Log("Attack time for: " + RequiredPlayer + ", skill " + RequiredInitiative);

            UpdateHelpInfo();
            Roster.HighlightShipsFiltered(FilterShipsToPerformAttack);

            IsReadyForCommands = true;
            Roster.GetPlayer(RequiredPlayer).PerformAttack();
        }

        private bool GetNextActivation(int pilotSkill)
        {

            bool result = false;

            var pilotSkillResults =
                from n in Roster.AllUnits
                where n.Value.State.Initiative == pilotSkill
                where n.Value.HasCombatActivation
                select n;

            if (pilotSkillResults.Count() > 0)
            {
                RequiredInitiative = pilotSkill;

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
                from n in Roster.AllUnits
                where n.Value.State.Initiative < pilotSkillMax
                where n.Value.HasCombatActivation
                orderby n.Value.State.Initiative
                select n;

            if (ascPilotSkills.Count() > 0)
            {
                result = ascPilotSkills.Last().Value.State.Initiative;
            }

            return result;
        }

        public override void FinishPhase()
        {
            if (Phases.Events.HasOnCombatPhaseEndEvents)
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

            if ((ship.Owner.PlayerNo == RequiredPlayer) && (ship.State.Initiative == RequiredInitiative) && (Roster.GetPlayer(RequiredPlayer).GetType() == typeof(Players.HumanPlayer)))
            {
                if (ship.IsAttackPerformed)
                {
                    Messages.ShowErrorToHuman("This ship cannot be selected:\nIt has already performed an attack");
                    return result;
                }
                result = true;
            }
            else
            {
                Messages.ShowErrorToHuman("This ship cannot be selected, the ship must be owned by " + Tools.PlayerToInt(RequiredPlayer) + " and have an initiative of " + RequiredInitiative);
            }

            return result;
        }

        public override void DoSelectThisShip(GenericShip ship, int mouseKeyIsPressed)
        {
            Roster.HighlightShipsFiltered(FilterShipsToAttack);

            GameMode.CurrentGameMode.ExecuteCommand(GenerateCombatActivationCommand(ship.ShipId));
        }

        public static GameCommand GenerateCombatActivationCommand(int shipId)
        {
            JSONObject parameters = new JSONObject();
            parameters.AddField("id", shipId.ToString());
            return GameController.GenerateGameCommand(
                GameCommandTypes.CombatActivation,
                typeof(CombatSubPhase),
                parameters.ToString()
            );
        }

        public static void DoCombatActivation(int shipId)
        {
            Selection.ChangeActiveShip("ShipId:" + shipId);

            Triggers.RegisterTrigger(
                new Trigger()
                {
                    Name = "Select a target to attack",
                    TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnSelectTargetForAttackStart_System,
                    EventHandler = StartCombatActivation
                }
            );

            Triggers.ResolveTriggers(
                TriggerTypes.OnSelectTargetForAttackStart_System,
                delegate { Phases.CurrentSubPhase.Next(); }
            );
        }

        private static void StartCombatActivation(object sender, System.EventArgs e)
        {
            Selection.ThisShip.CallCombatActivation(StartTargetSelection);
        }

        private static void StartTargetSelection()
        {
            if (!(Selection.ThisShip is GenericRemote))
            {
                Combat.StartSelectAttackTarget(
                    Selection.ThisShip,
                    Triggers.FinishTrigger
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool FilterShipsToAttack(GenericShip ship)
        {
            return ship.Owner.PlayerNo != RequiredPlayer;
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
                        result = true;
                    }
                    else
                    {
                        Messages.ShowErrorToHuman(targetShip.PilotInfo.PilotName + " cannot be selected as a target, it is a friendly ship");
                    }
                }
                else
                {
                    Messages.ShowErrorToHuman(targetShip.PilotInfo.PilotName + " cannot be selected as a target, first select the attacking ship");
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
                        Messages.ShowErrorToHuman("This ship already has attacked");
                    }
                }
            }
            return result;
        }

        private bool FilterShipsToPerformAttack(GenericShip ship)
        {
            return ship.State.Initiative == RequiredInitiative
                && ship.HasCombatActivation
                && ship.Owner.PlayerNo == RequiredPlayer;
        }

        public override void SkipButton()
        {
            if ((Selection.ThisShip == null) || (Selection.ThisShip.IsAttackPerformed))
            {
                if (TargetsArAvailable())
                {
                    AskToConfirmSkip();
                }
                else
                {
                    SkipCombatByShipsWithCurrentInitiative();
                }
            }
            else
            {
                GenericShip skippedShip = Selection.ThisShip;
                AfterSkippedCombatActivation(skippedShip);
                skippedShip.CallCombatDeactivation(CheckNext);
            }
        }

        private static bool TargetsArAvailable()
        {
            foreach (var ship in Roster.GetPlayer(Phases.CurrentPhasePlayer).Ships.Values)
            {
                if (ship.State.Initiative == Phases.CurrentSubPhase.RequiredInitiative && !ship.IsAttackPerformed)
                {
                    bool hasTarget = ActionsHolder.HasTarget(ship);
                    if (hasTarget) return true;
                }
            }

            return false;
        }

        private void AskToConfirmSkip()
        {
            AskToConfirmSkipSubphase subphase = Phases.StartTemporarySubPhaseNew<AskToConfirmSkipSubphase>(
                "Ask to confirm skip",
                delegate { }
            );

            subphase.DescriptionShort = "Are you sure?";
            subphase.DescriptionLong = "Some ships has targets to attack";

            subphase.AddDecision("Confirm", ConfirmSkip);
            subphase.AddDecision("Cancel", CancelSkip);

            subphase.ShowSkipButton = false;

            subphase.Start();
        }

        private void ConfirmSkip(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            SkipCombatByShipsWithCurrentInitiative();
        }

        private void CancelSkip(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            Next();
        }

        private class AskToConfirmSkipSubphase : DecisionSubPhase { };

        private void SkipCombatByShipsWithCurrentInitiative()
        {
            List<GenericShip> shipsToSkipCombat = new List<GenericShip>();

            foreach (var shipHolder in Roster.GetPlayer(Phases.CurrentPhasePlayer).Ships)
            {
                if (shipHolder.Value.State.Initiative == Phases.CurrentSubPhase.RequiredInitiative)
                {
                    shipsToSkipCombat.Add(shipHolder.Value);
                }
            }

            SkipCombatByShips(shipsToSkipCombat, Next);
        }

        private void SkipCombatByShips(List<GenericShip> shipsToSkipCombat, Action callback)
        {
            if (shipsToSkipCombat != null && shipsToSkipCombat.Count > 0)
            {
                GenericShip shipToSkipCombat = shipsToSkipCombat.First();

                shipsToSkipCombat.Remove(shipToSkipCombat);

                if (!shipToSkipCombat.IsAttackPerformed)
                {
                    Selection.ChangeActiveShip(shipToSkipCombat);
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
            ship.IsAttackPerformed = true;

            Selection.DeselectThisShip();
            Selection.DeselectAnotherShip();
            
            //TODO: From select target to select ship
        }

        private void CheckNext()
        {
            if (Roster.GetPlayer(RequiredPlayer).Ships.Count(n => n.Value.State.Initiative == RequiredInitiative && !n.Value.IsAttackPerformed) == 0)
            {
                Next();
            }
            else
            {
                Roster.HighlightShipsFiltered(FilterShipsToPerformAttack);

                IsReadyForCommands = true;
                Roster.GetPlayer(RequiredPlayer).PerformAttack();
            }
        }

        public override void DoSelectAnotherShip(GenericShip ship, int mouseKeyIsPressed)
        {
            if (mouseKeyIsPressed == 1)
            {
                if (Selection.ThisShip.IsAttackPerformed != true)
                {
                    UI.CheckFiringRangeAndShow();
                    UI.ClickDeclareTarget();
                }
                else
                {
                    Messages.ShowErrorToHuman("This ship already has attacked");
                }
            }
            else if (mouseKeyIsPressed == 2)
            {
                UI.CheckFiringRangeAndShow();
            }
        }
    }

}
