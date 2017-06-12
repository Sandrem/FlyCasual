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
            //BUG: If few these crits in the same time
            Selection.ActiveShip = Selection.ThisShip;
            Phases.StartTemporarySubPhase("Major Explosion", typeof(SubPhases.DiceRollSubPhase));

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

                DiceRoll damageRoll = new DiceRoll("attack", 0);
                damageRoll.DiceList.Add(new Dice("attack", DiceSide.Crit));
                host.SufferDamage(damageRoll);
            }

            Combat.ShowConfirmDiceResultsButton();
        }

        private void CloseWindow()
        {
            Phases.FinishSubPhase(typeof(SubPhases.DiceRollSubPhase));
            Combat.HideDiceResultMenu();
            Phases.Next();
        }

    }

}