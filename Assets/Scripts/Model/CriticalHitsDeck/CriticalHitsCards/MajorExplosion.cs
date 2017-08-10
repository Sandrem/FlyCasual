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
            ImageUrl = "http://i.imgur.com/6aASBM9.jpg";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Messages.ShowInfo("On a Hit result, suffer 1 critical damage.");
            Game.UI.AddTestLogEntry("On a Hit result, suffer 1 critical damage.");

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
            Phases.StartTemporarySubPhase(
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
            dicesType = DiceKind.Attack;
            dicesCount = 1;

            finishAction = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            if (CurrentDiceRoll.DiceList[0].Side == DiceSide.Success)
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
            Game.UI.AddTestLogEntry("Major Explosion: Suffer 1 additional critical damage");

            Selection.ActiveShip.AssignedDamageDiceroll.DiceList.Add(new Dice(DiceKind.Attack, DiceSide.Crit));

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Suffer critical damage",
                TriggerType = TriggerTypes.OnDamageIsDealt,
                TriggerOwner = Selection.ActiveShip.Owner.PlayerNo,
                EventHandler = Selection.ActiveShip.SufferDamage
            });

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, callBack);
        }

        private void NoDamage()
        {
            Messages.ShowInfo("No damage");
            callBack();
        }
    }

}