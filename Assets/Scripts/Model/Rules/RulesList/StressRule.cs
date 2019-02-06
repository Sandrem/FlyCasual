using UnityEngine;
using Ship;
using Tokens;
using ActionsList;
using Players;
using Movement;

namespace RulesList
{
    public class StressRule
    {

        public void PlanCheckStress(GenericShip ship)
        {
            if (BoardTools.Board.IsOffTheBoard(ship)) return;

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Check stress",
                TriggerOwner = ship.Owner.PlayerNo,
                TriggerType = TriggerTypes.OnMovementExecuted,
                EventHandler = CheckStress,
                Skippable = true
            });

            if (ship.Owner.UsesHotacAiRules)
            {
                ship.OnGenerateActions += AddRemoveStressActionForHotacAI;
            }
        }

        public static void CheckStress(object sender, System.EventArgs e)
        {
            switch (Selection.ThisShip.GetLastManeuverColor())
            {
                case MovementComplexity.Complex:
                    if (Selection.ThisShip.Owner.UsesHotacAiRules == false)
                    {
                        Selection.ThisShip.Tokens.AssignToken(typeof(StressToken), Triggers.FinishTrigger);
                    }
                    else
                    {
                        Selection.ThisShip.IsSkipsActionSubPhase = true;
                        Triggers.FinishTrigger();
                    }
                    break;
                case MovementComplexity.Easy:
                    if (Selection.ThisShip.Owner.UsesHotacAiRules == false)
                    {
                        Selection.ThisShip.Tokens.RemoveToken(
                            typeof(StressToken),
                            Triggers.FinishTrigger
                        );
                    }
                    else
                    {
                        Triggers.FinishTrigger();
                    }
                    break;
                default:
                    Triggers.FinishTrigger();
                    break;
            }
        }

        public void CanPerformActions(GenericAction action, ref bool result)
        {
            if (Selection.ThisShip.Tokens.GetToken(typeof(StressToken)) != null)
            {
                result = Selection.ThisShip.CanPerformActionsWhileStressed || action.CanBePerformedWhileStressed || Selection.ThisShip.ActionBar.ActionsThatCanbePreformedwhileStressed.Contains(action.GetType());
            }
        }

        public void CannotPerformRedManeuversWhileStressed(GenericShip ship, ref ManeuverHolder movement)
        {
            if ((movement.ColorComplexity == MovementComplexity.Complex) && (ship.Tokens.GetToken(typeof(StressToken)) != null))
            {
                if (!ship.CanPerformRedManeuversWhileStressed && !DirectionsMenu.IsForcedToShowRedManeuvers)
                {
                    movement.ColorComplexity = MovementComplexity.None;
                }
            }
        }

        private void AddRemoveStressActionForHotacAI(GenericShip host)
        {
            host.AddAvailableAction(new HotacRemoveStressAction() { HostShip = host });
        }

    }
}

namespace ActionsList
{

    public class HotacRemoveStressAction : GenericAction
    {

        public HotacRemoveStressAction()
        {
            Name = DiceModificationName = "Remove Stress";

            CanBePerformedWhileStressed = true;
        }

        public override void ActionTake()
        {
            HostShip.Tokens.RemoveToken(
                typeof(StressToken),
                Phases.CurrentSubPhase.CallBack
            );
        }

        public override int GetActionPriority()
        {
            int result = 0;
            if (HostShip.Tokens.HasToken(typeof(StressToken))) result = int.MaxValue;
            return result;
        }

    }

}

