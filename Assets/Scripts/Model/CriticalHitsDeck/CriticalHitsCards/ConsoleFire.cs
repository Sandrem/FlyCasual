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
            Triggers.RegisterTrigger(new Trigger() {
                Name = "#" + host.ShipId + ": Console Fire Crit",
                TriggerOwner = host.Owner.PlayerNo,
                TriggerType = TriggerTypes.OnCombatPhaseStart,
                EventHandler = RollForDamage
            });
        }

        private void RollForDamage(object sender, EventArgs e)
        {
            Selection.ActiveShip = Host;
            Phases.StartTemporarySubPhase(
                "Console Fire",
                typeof(SubPhases.ConsoleFireCheckSubPhase),
                delegate {
                    Phases.FinishSubPhase(typeof(SubPhases.ConsoleFireCheckSubPhase));
                    Triggers.FinishTrigger();
                });
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

            Name = "#" + Selection.ActiveShip.ShipId + ": " + Name;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            if (CurrentDiceRoll.DiceList[0].Side == DiceSide.Success)
            {
                SufferDamage();
            }
            else
            {
                NoDamage();
            }

            
        }

        private void SufferDamage()
        {
            Game.UI.ShowError("Console Fire: ship suffered damage");
            Game.UI.AddTestLogEntry("Console Fire: ship suffered damage");

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Suffer damage",
                TriggerType = TriggerTypes.OnDamageIsDealt,
                TriggerOwner = Selection.ActiveShip.Owner.PlayerNo,
                EventHandler = Selection.ActiveShip.SufferDamage
            });

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, callBack);
        }

        private void NoDamage()
        {
            Game.UI.ShowInfo("No damage");
            callBack();
        }
    }

}