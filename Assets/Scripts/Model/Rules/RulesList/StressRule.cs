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
            if (Board.BoardManager.IsOffTheBoard(ship)) return;

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Check stress",
                TriggerOwner = ship.Owner.PlayerNo,
                TriggerType = TriggerTypes.OnShipMovementExecuted,
                EventHandler = CheckStress
            });

            if (ship.Owner.GetType() == typeof(HotacAiPlayer))
            {
                ship.AfterGenerateAvailableActionsList += AddRemoveStressActionForHotacAI;
            }
        }

        public static void CheckStress(object sender, System.EventArgs e)
        {
            switch (Selection.ThisShip.GetLastManeuverColor())
            {
                case ManeuverColor.Red:
                    if (Selection.ThisShip.Owner.GetType() != typeof(HotacAiPlayer))
                    {
                        Selection.ThisShip.Tokens.AssignToken(new StressToken(Selection.ThisShip), delegate {
                            Selection.ThisShip.IsSkipsActionSubPhase = Selection.ThisShip.Tokens.HasToken(typeof(StressToken)) && !Selection.ThisShip.CanPerformActionsWhileStressed;
                            Triggers.FinishTrigger();
                        });
                    }
                    else
                    {
                        Selection.ThisShip.IsSkipsActionSubPhase = true;
                        Triggers.FinishTrigger();
                    }
                    break;
                case ManeuverColor.Green:
                    if (Selection.ThisShip.Owner.GetType() != typeof(HotacAiPlayer))
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
                result = Selection.ThisShip.CanPerformActionsWhileStressed || action.CanBePerformedWhileStressed;
            }
        }

        public void CannotPerformRedManeuversWhileStressed(GenericShip ship, ref MovementStruct movement)
        {
            if ((movement.ColorComplexity == ManeuverColor.Red) && (ship.Tokens.GetToken(typeof(StressToken)) != null))
            {
                if (!ship.CanPerformRedManeuversWhileStressed && !DirectionsMenu.IsForcedToShowRedManeuvers)
                {
                    movement.ColorComplexity = ManeuverColor.None;
                }
            }
        }

        private void AddRemoveStressActionForHotacAI(GenericShip host)
        {
            host.AddAvailableAction(new HotacRemoveStressAction() { Host = host });
        }

    }
}

namespace ActionsList
{

    public class HotacRemoveStressAction : GenericAction
    {

        public HotacRemoveStressAction()
        {
            Name = EffectName = "Remove Stress";

            CanBePerformedWhileStressed = true;
        }

        public override void ActionTake()
        {
            Host.Tokens.RemoveToken(
                typeof(StressToken),
                Phases.CurrentSubPhase.CallBack
            );
        }

        public override int GetActionPriority()
        {
            int result = 0;
            if (Host.Tokens.HasToken(typeof(StressToken))) result = int.MaxValue;
            return result;
        }

    }

}

