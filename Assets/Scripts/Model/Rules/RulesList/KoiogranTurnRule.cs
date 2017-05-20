using UnityEngine;

namespace RulesList
{
    public class KoiogranTurnRule
    {
        private GameManagerScript Game;

        public KoiogranTurnRule(GameManagerScript game)
        {
            Game = game;
        }

        public void CheckKoiogranTurn(Ship.GenericShip ship)
        {
            if (Game.Movement.CurrentMovementData.MovementBearing == ManeuverBearing.KoiogranTurn)
            {
                ship.Rotate180();
            }
        }

        public void CheckKoiogranTurnError(Ship.GenericShip ship)
        {
            if (Game.Movement.CurrentMovementData.MovementBearing == ManeuverBearing.KoiogranTurn)
            {
                Game.UI.ShowError("Koiogran Turn is failed due to collision");
            }
        }

    }
}
