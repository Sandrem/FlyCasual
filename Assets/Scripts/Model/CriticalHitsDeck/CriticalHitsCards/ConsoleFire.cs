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
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.OnCombatPhaseStart += PlanRollForDamage;
            Host.AfterGenerateAvailableActionsList += AddCancelCritAction;

            Host.AssignToken(new Tokens.ConsoleFireCritToken(), Triggers.FinishTrigger);
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
            Selection.ThisShip = Host;
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
            Messages.ShowError("Console Fire: ship suffered damage");

            Selection.ActiveShip.AssignedDamageDiceroll.DiceList.Add(CurrentDiceRoll.DiceList[0]);

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Suffer damage",
                TriggerType = TriggerTypes.OnDamageIsDealt,
                TriggerOwner = Selection.ActiveShip.Owner.PlayerNo,
                EventHandler = Selection.ActiveShip.SufferDamage,
                EventArgs = new DamageSourceEventArgs()
                {
                    Source = "Critical hit card",
                    DamageType = DamageTypes.CriticalHitCard
                }
            });

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, CallBack);
        }

        private void NoDamage()
        {
            Messages.ShowInfoToHuman("No damage");
            CallBack();
        }
    }

}