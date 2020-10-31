using Ship;
using Movement;

namespace RulesList
{
    public class PurpleManeuversRule
    {

        public void PlanCheckPurpleManeuversRule(GenericShip ship)
        {
            if (BoardTools.Board.IsOffTheBoard(ship)) return;

            if (Selection.ThisShip.GetLastManeuverColor() == MovementComplexity.Purple)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Check stress",
                    TriggerOwner = ship.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnMovementExecuted,
                    EventHandler = CheckStress,
                    Skippable = true
                });
            }
        }

        public static void CheckStress(object sender, System.EventArgs e)
        {
            Selection.ThisShip.State.Force--;
            Triggers.FinishTrigger();
        }
    }
}