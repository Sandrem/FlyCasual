using System;
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

            host.OnCombatPhaseStart += PlanRollForDamage;

            host.AfterGenerateAvailableActionsList += AddCancelCritAction;
        }

        private void PlanRollForDamage(Ship.GenericShip host)
        {
            Selection.ActiveShip = host;
            Triggers.AddTrigger("Console Fire Crit", TriggerTypes.OnCombatPhaseStart, RollForDamage);
        }

        private void RollForDamage(object sender, EventArgs e)
        {
            Phases.StartTemporarySubPhase("Console Fire", typeof(SubPhases.ConsoleFireCheckSubPhase));
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            host.RemoveToken(typeof(Tokens.ConsoleFireCritToken));

            host.OnCombatPhaseStart -= PlanRollForDamage;

            host.AfterGenerateAvailableActionsList -= AddCancelCritAction;
        }
    }

}

namespace SubPhases
{

    public class ConsoleFireCheckSubPhase : DiceRollCheckSubPhase
    {

        public override void Prepare()
        {
            dicesType = "attack";
            dicesCount = 1;

            checkResults = CheckResults;
        }

        protected override void CheckResults(DiceRoll diceRoll)
        {
            if (diceRoll.DiceList[0].Side == DiceSide.Success)
            {
                Game.UI.ShowError("Console Fire: ship suffered damage");
                Game.UI.AddTestLogEntry("Console Fire: ship suffered damage");
                Game.StartCoroutine(Selection.ActiveShip.SufferDamage(diceRoll));
            }

            base.CheckResults(diceRoll);
        }

    }

}