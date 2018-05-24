using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageDeckCardSE
{

    public class HullBreach : GenericDamageCard
    {
        public HullBreach()
        {
            Name = "Hull Breach";
            Type = CriticalCardType.Ship;
            ImageUrl = "https://i.imgur.com/CIqxoU5.jpg";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.OnSufferDamageDecidingSeverity += ChangeNormalDamageToCriticalDamage;
            Host.AfterGenerateAvailableActionsList += CallAddCancelCritAction;

            Host.Tokens.AssignCondition(new Tokens.HullBreachCritToken(Host));
            Triggers.FinishTrigger();
        }

        private void ChangeNormalDamageToCriticalDamage(object sender, EventArgs e, ref bool isCritical)
        {
            if (!isCritical)
            {
                Messages.ShowInfo("Hull Breach: Damage is suffered critical!");
                isCritical = true;
            }
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Messages.ShowInfo("Damage is suffered as usual");
            Host.Tokens.RemoveCondition(typeof(Tokens.HullBreachCritToken));

            Host.OnSufferDamageDecidingSeverity -= ChangeNormalDamageToCriticalDamage;
            Host.AfterGenerateAvailableActionsList -= CallAddCancelCritAction;
        }

    }

}