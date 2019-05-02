using GameModes;
using Ship;
using SubPhases;
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

        public override void AssignManeuver()
        {
            base.AssignManeuver();

            AssignManeuverRecursive();
        }

        private void AssignManeuverRecursive()
        {
            GenericShip shipWithoutManeuver = Ships.Values
                .Where(n => !n.State.IsIonized)
                .FirstOrDefault(n => n.AssignedManeuver == null);

            if (shipWithoutManeuver != null)
            {
                Selection.ChangeActiveShip(shipWithoutManeuver);
                AI.Aggressor.NavigationSubSystem.CalculateNavigation(Selection.ThisShip, FinishAssignManeuver);
            }
            else
            {
                GameMode.CurrentGameMode.ExecuteCommand(UI.GenerateNextButtonCommand());
            }
        }

        private void FinishAssignManeuver()
        {
            ShipMovementScript.SendAssignManeuverCommand(Selection.ThisShip.ShipId, AI.Aggressor.NavigationSubSystem.BestManeuver);

            AssignManeuverRecursive();
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
    }
}
