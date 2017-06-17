using UnityEngine;

namespace RulesList
{
    public class AsteroidHitRule
    {
        private GameManagerScript Game;

        public AsteroidHitRule(GameManagerScript game)
        {
            Game = game;
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            Phases.OnActionSubPhaseStart += CheckSkipPerformAction;
        }

        public void CheckSkipPerformAction()
        {
            if (Selection.ThisShip.ObstaclesHit.Count > 0)
            {
                Game.UI.ShowError("Hit asteroid during movement - action subphase is skipped");
                Selection.ThisShip.IsSkipsActionSubPhase = true;
            }
        }

        public void CheckDamage(Ship.GenericShip ship)
        {
            if (Selection.ThisShip.ObstaclesHit.Count > 0)
            {
                Game.UI.ShowError("Hit asteroid during movement - rolling for damage");

                Selection.ActiveShip = Selection.ThisShip;
                foreach (var asteroid in Selection.ThisShip.ObstaclesHit)
                {
                    Phases.StartTemporarySubPhase("Damage from asteroid collision", typeof(SubPhases.AsteroidHitCheckSubPhase));
                }
            }
        }
    }
}

namespace SubPhases
{

    public class AsteroidHitCheckSubPhase : DiceRollCheckSubPhase
    {

        public override void Prepare()
        {
            dicesType = "attack";
            dicesCount = 1;

            checkResults = CheckResults;
        }

        protected override void CheckResults(DiceRoll diceRoll)
        {
            switch (diceRoll.DiceList[0].Side)
            {
                case DiceSide.Blank:
                    Game.UI.ShowInfo("No damage");
                    break;
                case DiceSide.Focus:
                    Game.UI.ShowInfo("No damage");
                    break;
                case DiceSide.Success:
                    Game.UI.ShowError("Damage is dealt!");
                    Selection.ThisShip.SufferDamage(diceRoll);
                    break;
                case DiceSide.Crit:
                    Game.UI.ShowError("Critical damage is dealt!");
                    Selection.ThisShip.SufferDamage(diceRoll);
                    break;
                default:
                    break;
            }

            base.CheckResults(diceRoll);
        }

    }

}
