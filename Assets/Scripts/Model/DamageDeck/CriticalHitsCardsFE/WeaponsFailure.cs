using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageDeckCardFE
{

    public class WeaponsFailure : GenericDamageCard
    {
        public WeaponsFailure()
        {
            Name = "Weapons Failure";
            Type = CriticalCardType.Ship;
            CancelDiceResults.Add(DieSide.Success);
            CancelDiceResults.Add(DieSide.Crit);
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.AfterGotNumberOfAttackDice += ReduceNumberOfAttackDice;
            Host.OnGenerateActions += CallAddCancelCritAction;

            Host.Tokens.AssignCondition(typeof(Tokens.WeaponsFailureCritToken));
            Triggers.FinishTrigger();
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Messages.ShowInfo("Number of attack dice is restored");

            Host.Tokens.RemoveCondition(typeof(Tokens.WeaponsFailureCritToken));
            Host.AfterGotNumberOfAttackDice -= ReduceNumberOfAttackDice;
            Host.OnGenerateActions -= CallAddCancelCritAction;
        }

        private void ReduceNumberOfAttackDice(ref int value)
        {
            Messages.ShowInfo("Weapons Failure: Number of attack dice is reduced");

            value--;
        }

    }

}