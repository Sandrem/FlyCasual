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
            if (Selection.ThisShip.AssignedManeuver.Bearing == Movement.ManeuverBearing.KoiogranTurn)
            {
                if (!Selection.ThisShip.IsBumped)
                {
                    Phases.StartTemporarySubPhase("Koiogran Turn", typeof(SubPhases.KoiogranTurnSubPhase));
                }
                else
                {
                    Game.UI.ShowError("Koiogran Turn is failed due to collision");
                }
                
            }
        }

    }
}
