using System;
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

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Selection.ActiveShip = Host;

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Roll for asteroid damage",
                TriggerOwner = Selection.ActiveShip.Owner.PlayerNo,
                TriggerType = TriggerTypes.OnMajorExplosionCrit,
                EventHandler = RollForDamage
            });

            Triggers.ResolveTriggers(TriggerTypes.OnMajorExplosionCrit, delegate { Triggers.FinishTrigger(); });
        }

        private void RollForDamage(object sender, EventArgs e)
        {
            Phases.StartTemporarySubPhaseOld(
                "Major Explosion",
                typeof(SubPhases.MajorExplosionCheckSubPhase),
                delegate {
                    Phases.FinishSubPhase(typeof(SubPhases.MajorExplosionCheckSubPhase));
                    Triggers.FinishTrigger();
                });
        }

    }

}

namespace SubPhases
{

    public class MajorExplosionCheckSubPhase : DiceRollCheckSubPhase
    {

        public override void Prepare()
        {
            diceType = DiceKind.Attack;
            diceCount = 1;

            finishAction = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            if (CurrentDiceRoll.DiceList[0].Side == DieSide.Success)
            {
                DealDamage();
            }
            else
            {
                NoDamage();
            }
        }

        private void DealDamage()
        {
            Messages.ShowInfo("Major Explosion: Suffer 1 additional critical damage");

            Selection.ActiveShip.AssignedDamageDiceroll.AddDice(DieSide.Crit);

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Suffer critical damage",
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
            Messages.ShowInfo("No damage");
            CallBack();
        }
    }

}