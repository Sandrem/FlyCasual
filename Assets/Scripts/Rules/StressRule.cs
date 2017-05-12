
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
                    ship.AddToken(new Tokens.StressToken());
                    break;
                case Ship.ManeuverColor.Green:
                    ship.RemoveToken(new Tokens.StressToken());
                    break;
            }
        }

        public void CanPerformActions(ref bool result, bool afterMovement)
        {
            if (Game.Selection.ThisShip.HasToken(new Tokens.StressToken())) result = false;
        }

        public void CannotPerformRedManeuversWhileStressed(Ship.GenericShip ship, ref Movement movement)
        {
            //TODO: Should I show red maneuvers if I have stress?
            if ((movement.ColorComplexity == Ship.ManeuverColor.Red) && (ship.HasToken(new Tokens.StressToken())))
            {
                movement.ColorComplexity = Ship.ManeuverColor.None;
            }
        }

    }
}
