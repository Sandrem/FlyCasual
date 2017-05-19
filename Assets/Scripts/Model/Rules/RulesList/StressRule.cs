using UnityEngine;

namespace RulesList
{
    public class StressRule
    {
        private GameManagerScript Game;

        public StressRule(GameManagerScript game)
        {
            Game = game;
        }

        public void CheckStress(Ship.GenericShip ship)
        {
            switch (ship.GetLastManeuverColor())
            {
                case Ship.ManeuverColor.Red:
                    ship.AssignToken(new Tokens.StressToken());
                    break;
                case Ship.ManeuverColor.Green:
                    ship.RemoveToken(typeof(Tokens.StressToken));
                    break;
            }
        }

        public void CanPerformActions(ActionsList.GenericAction action, ref bool result)
        {
            if (Game.Selection.ThisShip.GetToken(typeof(Tokens.StressToken)) != null)
            {
                result = false;
            }
        }

        public void CannotPerformRedManeuversWhileStressed(Ship.GenericShip ship, ref Movement movement)
        {
            //TODO: Should I show red maneuvers if I have stress?
            if ((movement.ColorComplexity == Ship.ManeuverColor.Red) && (ship.GetToken(typeof(Tokens.StressToken)) != null))
            {
                movement.ColorComplexity = Ship.ManeuverColor.None;
            }
        }

    }
}
