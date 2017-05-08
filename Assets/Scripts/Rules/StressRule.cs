
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
            switch (ship.GetLastManeurColor())
            {
                case Ship.ManeuverColor.Red:
                    ship.AssignStressToken();
                    break;
                case Ship.ManeuverColor.Green:
                    ship.TryRemoveStressToken();
                    break;
            }
        }

        public void CanPerformActions(ref bool result, bool afterMovement)
        {
            if (Game.Selection.ThisShip.HasToken(Ship.Token.Stress)) result = false;
        }

    }
}
