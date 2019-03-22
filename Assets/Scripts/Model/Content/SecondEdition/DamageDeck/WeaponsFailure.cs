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
            ImageUrl = "https://i.imgur.com/RyNw8Pj.png";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.AfterGotNumberOfAttackDice += ReduceNumberOfAttackDice;
            Host.OnGenerateActions += CallAddCancelCritAction;

            Host.Tokens.AssignCondition(typeof(Tokens.WeaponsFailureSECritToken));
            Triggers.FinishTrigger();
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Messages.ShowInfo("Weapons Failure has been repaired.  " + Host.PilotInfo.PilotName + "'s attack dice have been restored.");

            Host.Tokens.RemoveCondition(typeof(Tokens.WeaponsFailureSECritToken));
            Host.AfterGotNumberOfAttackDice -= ReduceNumberOfAttackDice;
            Host.OnGenerateActions -= CallAddCancelCritAction;
        }

        private void ReduceNumberOfAttackDice(ref int value)
        {
            Messages.ShowInfo("Weapons Failure: " + Host.PilotInfo.PilotName + "'s attack dice have been reduced by 1.");

            value--;
        }

    }

}