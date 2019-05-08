using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageDeckCardFE
{
    public class ConsoleFire : GenericDamageCard
    {
        public ConsoleFire()
        {
            Name = "Console Fire";
            Type = CriticalCardType.Ship;
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Phases.Events.OnCombatPhaseStart_Triggers += PlanRollForDamage;
            Host.OnGenerateActions += CallAddCancelCritAction;
            Host.OnShipIsDestroyed += delegate { DiscardEffect(); };

            Host.Tokens.AssignCondition(typeof(Tokens.ConsoleFireCritToken));
            Triggers.FinishTrigger();
        }

        protected virtual void PlanRollForDamage()
        {
            Triggers.RegisterTrigger(new Trigger() {
                Name = "#" + Host.ShipId + ": Console Fire Crit",
                TriggerOwner = Host.Owner.PlayerNo,
                TriggerType = TriggerTypes.OnCombatPhaseStart,
                EventHandler = RollForDamage
            });
        }

        protected void RollForDamage(object sender, EventArgs e)
        {
            Selection.ActiveShip = Host;
            Selection.ThisShip = Host;
            Phases.StartTemporarySubPhaseOld(
                "Console Fire",
                typeof(SubPhases.ConsoleFireCheckSubPhase),
                delegate {
                    Phases.FinishSubPhase(typeof(SubPhases.ConsoleFireCheckSubPhase));
                    Triggers.FinishTrigger();
                });
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Host.Tokens.RemoveCondition(typeof(Tokens.ConsoleFireCritToken));

            Phases.Events.OnCombatPhaseStart_Triggers -= PlanRollForDamage;

            Host.OnGenerateActions -= CallAddCancelCritAction;
        }
    }

}

namespace SubPhases
{

    public class ConsoleFireCheckSubPhase : DiceRollCheckSubPhase
    {

        public override void Prepare()
        {
            DiceKind = DiceKind.Attack;
            DiceCount = 1;

            AfterRoll = FinishAction;

            Name = "#" + Selection.ActiveShip.ShipId + ": " + Name;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            if (CurrentDiceRoll.DiceList[0].Side == DieSide.Success)
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
            Messages.ShowInfo("Console Fire: This ship suffered 1 damage");

            DamageSourceEventArgs consolefireDamage = new DamageSourceEventArgs()
            {
                Source = "Critical hit card",
                DamageType = DamageTypes.CriticalHitCard
            };

            Selection.ActiveShip.Damage.TryResolveDamage(1, consolefireDamage, CallBack);
        }

        private void NoDamage()
        {
            Messages.ShowInfoToHuman("Console Fire: No damage was suffered");
            CallBack();
        }
    }

}