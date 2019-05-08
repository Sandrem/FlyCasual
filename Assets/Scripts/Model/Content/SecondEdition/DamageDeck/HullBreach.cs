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
            ImageUrl = "https://i.imgur.com/6CnuFDH.png";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.OnSufferDamageDecidingSeverity += ChangeNormalDamageToCriticalDamage;
            Host.OnGenerateActions += CallAddCancelCritAction;

            Host.Tokens.AssignCondition(typeof(Tokens.HullBreachCritToken));
            Triggers.FinishTrigger();
        }

        private void ChangeNormalDamageToCriticalDamage(object sender, EventArgs e, ref bool isCritical)
        {
            if (!isCritical)
            {
                Messages.ShowInfo("Due to a Hull Breach, the ship has suffered a Critical Hit instead of a normal Hit!");
                isCritical = true;
            }
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Messages.ShowInfo("Hull Breach has been repaired, Hits are no longer upgraded to Critical Hits against " + Host.PilotInfo.PilotName);
            Host.Tokens.RemoveCondition(typeof(Tokens.HullBreachCritToken));

            Host.OnSufferDamageDecidingSeverity -= ChangeNormalDamageToCriticalDamage;
            Host.OnGenerateActions -= CallAddCancelCritAction;
        }

    }

}