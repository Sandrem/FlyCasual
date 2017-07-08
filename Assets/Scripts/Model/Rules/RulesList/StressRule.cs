using UnityEngine;

namespace RulesList
{
    public class StressRule
    {

        public void CheckStress(Ship.GenericShip ship)
        {
            switch (ship.GetLastManeuverColor())
            {
                case Movement.ManeuverColor.Red:
                    ship.AssignToken(new Tokens.StressToken());
                    ship.IsSkipsActionSubPhase = true;
                    break;
                case Movement.ManeuverColor.Green:
                    if (ship.Owner.GetType() != typeof(Players.HotacAiPlayer))
                    {
                        ship.RemoveToken(typeof(Tokens.StressToken));
                    }                    
                    break;
            }
        }

        public void CanPerformActions(ActionsList.GenericAction action, ref bool result)
        {
            if (Selection.ThisShip.GetToken(typeof(Tokens.StressToken)) != null)
            {
                result = false;
            }
        }

        public void CannotPerformRedManeuversWhileStressed(Ship.GenericShip ship, ref Movement.MovementStruct movement)
        {
            //TODO: Should I show red maneuvers if I have stress?
            if ((movement.ColorComplexity == Movement.ManeuverColor.Red) && (ship.GetToken(typeof(Tokens.StressToken)) != null))
            {
                movement.ColorComplexity = Movement.ManeuverColor.None;
            }
        }

    }
}
