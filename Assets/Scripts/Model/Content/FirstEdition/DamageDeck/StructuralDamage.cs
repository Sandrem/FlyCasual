﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageDeckCardFE
{

    public class StructuralDamage : GenericDamageCard
    {
        public StructuralDamage()
        {
            Name = "Structural Damage";
            Type = CriticalCardType.Ship;
            CancelDiceResults.Add(DieSide.Success);
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Messages.ShowInfo("Agility is reduced");

            Host.ChangeAgilityBy(-1);
            Host.OnGenerateActions += CallAddCancelCritAction;
            Host.Tokens.AssignCondition(typeof(Tokens.StructuralDamageCritToken));
            Triggers.FinishTrigger();
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Messages.ShowInfo("Agility is restored");

            Host.Tokens.RemoveCondition(typeof(Tokens.StructuralDamageCritToken));
            Host.OnGenerateActions -= CallAddCancelCritAction;
            Host.ChangeAgilityBy(+1);
        }

        private void ReduceAgility(ref int value)
        {
            value--;
        }

    }

}