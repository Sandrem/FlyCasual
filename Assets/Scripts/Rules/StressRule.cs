
using UnityEngine;

namespace Rules
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

        public void CanPerformActions(Actions.GenericAction action, ref bool result)
        {
            if (Game.Selection.ThisShip.HasToken(typeof(Tokens.StressToken)))
            {
                result = false;
            }
        }

        public void CannotPerformRedManeuversWhileStressed(Ship.GenericShip ship, ref Movement movement)
        {
            //TODO: Should I show red maneuvers if I have stress?
            if ((movement.ColorComplexity == Ship.ManeuverColor.Red) && (ship.HasToken(typeof(Tokens.StressToken))))
            {
                movement.ColorComplexity = Ship.ManeuverColor.None;
            }
        }

    }
}
