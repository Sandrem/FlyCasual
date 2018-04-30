using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageDeckCard
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
            Phases.OnCombatPhaseStart_Triggers += PlanRollForDamage;
            Host.AfterGenerateAvailableActionsList += CallAddCancelCritAction;
            Host.OnShipIsDestroyed += delegate { DiscardEffect(); };

            Host.Tokens.AssignCondition(new Tokens.ConsoleFireCritToken(Host));
            Triggers.FinishTrigger();
        }

        private void PlanRollForDamage()
        {
            Triggers.RegisterTrigger(new Trigger() {
                Name = "#" + Host.ShipId + ": Console Fire Crit",
                TriggerOwner = Host.Owner.PlayerNo,
                TriggerType = TriggerTypes.OnCombatPhaseStart,
                EventHandler = RollForDamage
            });
        }

        private void RollForDamage(object sender, EventArgs e)
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
            Host.Tokens.RemoveCondition(typeof(Tokens.ConsoleFireCritToken));

            Phases.OnCombatPhaseStart_Triggers -= PlanRollForDamage;

            Host.AfterGenerateAvailableActionsList -= CallAddCancelCritAction;
        }
    }

}

namespace SubPhases
{

    public class ConsoleFireCheckSubPhase : DiceRollCheckSubPhase
    {

        public override void Prepare()
        {
            diceType = DiceKind.Attack;
            diceCount = 1;

            finishAction = FinishAction;

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