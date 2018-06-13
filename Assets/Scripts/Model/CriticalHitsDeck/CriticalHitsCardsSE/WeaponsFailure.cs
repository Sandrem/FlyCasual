using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageDeckCardSE
{

    public class WeaponsFailure : GenericDamageCard
    {
        public WeaponsFailure()
        {
            Name = "Weapons Failure";
            Type = CriticalCardType.Ship;
            ImageUrl = "https://i.imgur.com/BTd2jSS.png";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.AfterGotNumberOfAttackDice += ReduceNumberOfAttackDice;
            Host.AfterGenerateAvailableActionsList += CallAddCancelCritAction;

            Host.Tokens.AssignCondition(typeof(Tokens.WeaponsFailureSECritToken));
            Triggers.FinishTrigger();
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Messages.ShowInfo("Number of attack dice is restored");

            Host.Tokens.RemoveCondition(typeof(Tokens.WeaponsFailureSECritToken));
            Host.AfterGotNumberOfAttackDice -= ReduceNumberOfAttackDice;
            Host.AfterGenerateAvailableActionsList -= CallAddCancelCritAction;
        }

        private void ReduceNumberOfAttackDice(ref int value)
        {
            Messages.ShowInfo("Weapons Failure: Number of attack dice is reduced");

            value--;
        }

    }

}