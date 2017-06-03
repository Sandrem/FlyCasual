using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CriticalHitCard
{

    public class MajorExplosion : GenericCriticalHit
    {
        public MajorExplosion()
        {
            Name = "Major Explosion";
            Type = CriticalCardType.Ship;
        }

        public override void ApplyEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("Roll 1 attack die. On a Hit result, suffer 1 critical damage.");
            Game.UI.AddTestLogEntry("Roll 1 attack die. On a Hit result, suffer 1 critical damage.");
            RollForDamage(host);
        }

        private void RollForDamage(Ship.GenericShip host)
        {
            //TODO: StartSubhase

            Combat.ShowDiceResultMenu(CloseWindow);

            DiceRoll DiceRollCheck;
            DiceRollCheck = new DiceRoll("attack", 1);
            DiceRollCheck.Roll();
            DiceRollCheck.CalculateResults(CheckResults);
        }

        private void CheckResults(DiceRoll diceRoll)
        {
            Combat.CurentDiceRoll = diceRoll;

            if (diceRoll.DiceList[0].Side == DiceSide.Success)
            {
                Game.UI.ShowError("Major Explosion: Suffer 1 additional critical damage");
                Game.UI.AddTestLogEntry("Major Explosion: Suffer 1 additional critical damage");
                host.SufferGenericDamage(1);
            }

            Combat.ShowConfirmDiceResultsButton();
        }

        private void CloseWindow()
        {
            //TODO: EndSubhase
            Combat.HideDiceResultMenu();
        }

    }

}