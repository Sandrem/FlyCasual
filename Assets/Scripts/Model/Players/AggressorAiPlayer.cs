using GameModes;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Players
{

    public partial class AggressorAiPlayer : GenericAiPlayer
    {

        public AggressorAiPlayer() : base()
        {
            Name = "Aggressor AI";

            NickName = "Aggressor";
            Title = "Assassin Droid";
            Avatar = "UpgradesList.FirstEdition.IG88D";
        }

        public override void AssignManeuversStart()
        {
            base.AssignManeuversStart();

            if (!DebugManager.DebugStraightToCombat)
            {
                CalculateNavigation();
            }
            else
            {
                AssignManeuversRecursive();
            }
        }

        private void CalculateNavigation()
        {
            AI.Aggressor.NavigationSubSystem.CalculateNavigation(AssignManeuversRecursive);
        }

        private void AssignManeuversRecursive()
        {
            GenericShip shipWithoutManeuver = (!DebugManager.DebugStraightToCombat) ? AI.Aggressor.NavigationSubSystem.GetNextShipWithoutAssignedManeuver() : GetNextShipWithoutAssignedManeuver();

            if (shipWithoutManeuver != null)
            {
                Selection.ChangeActiveShip(shipWithoutManeuver);
                OpenDirectionsUiSilent();
            }
            else
            {
                GameMode.CurrentGameMode.ExecuteCommand(UI.GenerateNextButtonCommand());
            }
        }

        private GenericShip GetNextShipWithoutAssignedManeuver()
        {
            return Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).Ships.Values
                .Where(n => n.AssignedManeuver == null && !n.State.IsIonized)
                .FirstOrDefault();
        }

        private void OpenDirectionsUiSilent()
        {
            GameMode.CurrentGameMode.ExecuteCommand(
                PlanningSubPhase.GenerateSelectShipToAssignManeuver(Selection.ThisShip.ShipId)
            );
        }

        public override void AskAssignManeuver()
        {
            if (!DebugManager.DebugStraightToCombat)
            {
                AI.Aggressor.NavigationSubSystem.AssignPlannedManeuver(AssignManeuversRecursive);
            }
            else
            {
                ShipMovementScript.SendAssignManeuverCommand("2.F.S");
                AssignManeuversRecursive();
            }
        }

        protected override GenericShip SelectTargetForAttack()
        {
            if (DebugManager.DebugNoCombat) return null;

            return AI.Aggressor.TargetingSubSystem.SelectTargetAndWeapon(Selection.ThisShip);
        }

        protected override void PerformActionFromList(List<ActionsList.GenericAction> actionsList)
        {
            bool isActionTaken = false;

            List<ActionsList.GenericAction> availableActionsList = actionsList;

            Dictionary<ActionsList.GenericAction, int> actionsPriority = new Dictionary<ActionsList.GenericAction, int>();

            foreach (var action in availableActionsList)
            {
                int priority = action.GetActionPriority();
                Selection.ThisShip.Ai.CallGetActionPriority(action, ref priority);

                //Do not perform red action if this is not rotate arc action
                if (action.IsRed && !(action is ActionsList.RotateArcAction)) priority = int.MinValue;

                actionsPriority.Add(action, priority);
            }

            actionsPriority = actionsPriority.OrderByDescending(n => n.Value).ToDictionary(n => n.Key, n => n.Value);

            if (actionsPriority.Count > 0)
            {
                KeyValuePair<ActionsList.GenericAction, int> prioritizedActions = actionsPriority.First();

                if (prioritizedActions.Value > 0)
                {
                    isActionTaken = true;

                    //Actions.TakeActionStart(prioritizedActions.Key);
                    JSONObject parameters = new JSONObject();
                    parameters.AddField("name", prioritizedActions.Key.Name);
                    GameController.SendCommand(
                        GameCommandTypes.Decision,
                        Phases.CurrentSubPhase.GetType(),
                        parameters.ToString()
                    );
                }
            }

            if (!isActionTaken)
            {
                GameMode.CurrentGameMode.ExecuteCommand(UI.GenerateSkipButtonCommand());
            }
        }

        public override void SetupShip()
        {
            Roster.HighlightPlayer(PlayerNo);
            GameController.CheckExistingCommands();

            AI.Aggressor.DeploymentSubSystem.SetupShip();
        }

        public override void PerformManeuver()
        {
            Roster.HighlightPlayer(PlayerNo);
            GameController.CheckExistingCommands();

            GenericShip nextShip = (!DebugManager.DebugStraightToCombat) ? AI.Aggressor.NavigationSubSystem.GetNextShipWithoutFinishedManeuver() : GetNextShipWithoutFinishedManeuver();
            if (nextShip != null)
            {
                Selection.ChangeActiveShip("ShipId:" + nextShip.ShipId);
                ActivateShip(nextShip);
            }
            else
            {
                Phases.Next();
            }
        }

        private static GenericShip GetNextShipWithoutFinishedManeuver()
        {
            return Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).Ships.Values
                .Where(n => !n.IsManeuverPerformed)
                .FirstOrDefault();
        }
    }
}
