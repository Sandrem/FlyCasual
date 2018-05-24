using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageDeckCardFE
{

    public class DirectHit : GenericDamageCard
    {
        public DirectHit()
        {
            Name = "Direct Hit";
            Type = CriticalCardType.Ship;
            AiAvoids = true;

            DamageValue = 2;
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.Tokens.AssignCondition(new Tokens.DirectHitCritToken(Host));
            AdditionalHullDamage();
        }

        private void AdditionalHullDamage()
        {
            Host.CallHullValueIsDecreased(Triggers.FinishTrigger);
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Host.Tokens.RemoveCondition(typeof(Tokens.DirectHitCritToken));
            Host.CallAfterAssignedDamageIsChanged();
            Messages.ShowInfo("One hull point is restored");
        }
    }

}