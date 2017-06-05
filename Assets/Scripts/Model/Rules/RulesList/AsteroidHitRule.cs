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
                Selection.ThisShip.IsSkipsAction = true;
            }
        }

        public void CheckDamage(Ship.GenericShip ship)
        {
            if (Selection.ThisShip.ObstaclesHit.Count > 0)
            {
                Game.UI.ShowError("Hit asteroid during movement - rolling for damage");

                //Todo: Throw visual dice

                foreach (var asteroid in Selection.ThisShip.ObstaclesHit)
                {
                    int diceResult = Random.Range(0, 8);
                    DiceRoll newDiceroll = new DiceRoll("attack", 0);
                    if (diceResult < 5)
                    {
                        Game.UI.ShowInfo("Roll result: No damage");
                    }
                    else if (diceResult > 7)
                    {
                        Dice newDice = new Dice("attack", DiceSide.Crit);
                        newDiceroll.DiceList.Add(newDice);
                        Game.UI.ShowError("Roll result: Critical hit!");
                        Selection.ThisShip.SufferDamage(newDiceroll);
                    }
                    else
                    {
                        Dice newDice = new Dice("attack", DiceSide.Success);
                        newDiceroll.DiceList.Add(newDice);
                        Game.UI.ShowError("Roll result: Hit!");
                        Selection.ThisShip.SufferDamage(newDiceroll);
                    }
                }

            }
        }

    }
}
