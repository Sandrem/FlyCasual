using UnityEngine;

namespace RulesList
{
    public class StressRule
    {

        public void PlanCheckStress(Ship.GenericShip ship)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Check stress",
                TriggerOwner = ship.Owner.PlayerNo,
                TriggerType = TriggerTypes.OnShipMovementExecuted,
                EventHandler = CheckStress
            });
        }

        private static void CheckStress(object sender, System.EventArgs e)
        {
            switch (Selection.ThisShip.GetLastManeuverColor())
            {
                case Movement.ManeuverColor.Red:
                    if (Selection.ThisShip.Owner.GetType() != typeof(Players.HotacAiPlayer))
                    {
                        Selection.ThisShip.AssignToken(new Tokens.StressToken(), delegate {
                            Selection.ThisShip.IsSkipsActionSubPhase = Selection.ThisShip.HasToken(typeof(Tokens.StressToken)) && !Selection.ThisShip.CanPerformActionsWhileStressed;
                            Triggers.FinishTrigger();
                        });
                    }
                    else
                    {
                        Selection.ThisShip.IsSkipsActionSubPhase = true;
                        Triggers.FinishTrigger();
                    }
                    break;
                case Movement.ManeuverColor.Green:
                    if (Selection.ThisShip.Owner.GetType() != typeof(Players.HotacAiPlayer))
                    {
                        Selection.ThisShip.RemoveToken(typeof(Tokens.StressToken));
                    }
                    Triggers.FinishTrigger();
                    break;
                default:
                    Triggers.FinishTrigger();
                    break;
            }
        }

        public void CanPerformActions(ActionsList.GenericAction action, ref bool result)
        {
            if (Selection.ThisShip.GetToken(typeof(Tokens.StressToken)) != null)
            {
                result = Selection.ThisShip.CanPerformActionsWhileStressed;
            }
        }

        public void CannotPerformRedManeuversWhileStressed(Ship.GenericShip ship, ref Movement.MovementStruct movement)
        {
            //TODO: Should I show red maneuvers if I have stress?
            if ((movement.ColorComplexity == Movement.ManeuverColor.Red) && (ship.GetToken(typeof(Tokens.StressToken)) != null))
            {
                if (!ship.CanPerformRedManeuversWhileStressed && !DirectionsMenu.ForceShowRedManeuvers)
                {
                    movement.ColorComplexity = Movement.ManeuverColor.None;
                }
            }
        }

    }
}
