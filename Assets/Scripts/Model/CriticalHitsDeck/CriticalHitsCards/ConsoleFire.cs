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
            ImageUrl = "http://i.imgur.com/Jamd8dB.jpg";
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
            Triggers.RegisterTrigger(new Trigger() { Name = "#" + host.ShipId + ": Console Fire Crit", TriggerOwner = host.Owner.PlayerNo, triggerType = TriggerTypes.OnCombatPhaseStart, eventHandler = RollForDamage });
            //OldTriggers.AddTrigger("#" + host.ShipId + ": Console Fire Crit", TriggerTypes.OnCombatPhaseStart, RollForDamage, host, host.Owner.PlayerNo);
        }

        private void RollForDamage(object sender, EventArgs e)
        {
            Selection.ActiveShip = Host;
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
            dicesType = DiceKind.Attack;
            dicesCount = 1;

            finishAction = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            if (CurrentDiceRoll.DiceList[0].Side == DiceSide.Success)
            {
                Game.UI.ShowError("Console Fire: ship suffered damage");
                Game.UI.AddTestLogEntry("Console Fire: ship suffered damage");

                DamageSourceEventArgs eventArgs = new DamageSourceEventArgs();
                eventArgs.Source = new CriticalHitCard.ConsoleFire();
                eventArgs.DamageType = DamageTypes.CriticalHitCard;
                Selection.ActiveShip.SufferDamage(CurrentDiceRoll, eventArgs);
            }

            Phases.FinishSubPhase(this.GetType());
        }

    }

}