using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CriticalHitCard
{

    public class ConsoleFire : GenericCriticalHit
    {
        public ConsoleFire()
        {
            Name = "Console Fire";
            Type = CriticalCardType.Ship;
            ImageUrl = "http://i.imgur.com/RwtlPpG.jpg";
        }

        public override void ApplyEffect(Ship.GenericShip host)
        {
            
            Game.UI.ShowInfo("At the start of each Combat phase, roll 1 attack die.");
            Game.UI.AddTestLogEntry("At the start of each Combat phase, roll 1 attack die.");
            host.AssignToken(new Tokens.ConsoleFireCritToken());

            host.OnCombatPhaseStart += RollForDamage;

            host.AfterAvailableActionListIsBuilt += AddCancelCritAction;
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
                Game.UI.ShowError("Console Fire: ship suffered damage");
                Game.UI.AddTestLogEntry("Console Fire: ship suffered damage");
                host.SufferGenericDamage(1);
            }
            
            Combat.ShowConfirmDiceResultsButton();
        }

        private void CloseWindow()
        {
            //TODO: EndSubhase
            Combat.HideDiceResultMenu();
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            host.RemoveToken(typeof(Tokens.ConsoleFireCritToken));

            host.OnCombatPhaseStart -= RollForDamage;

            host.AfterAvailableActionListIsBuilt -= AddCancelCritAction;
        }
    }

}