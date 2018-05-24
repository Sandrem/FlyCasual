using ActionsList;
using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageDeckCardSE
{

    public class DamagedSensorArray : GenericDamageCard
    {
        public DamagedSensorArray()
        {
            Name = "Damaged Sensor Array";
            Type = CriticalCardType.Ship;
            ImageUrl = "https://i.imgur.com/6r6a8a7.jpg";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.OnTryAddAvailableAction += OnlyCancelCritActionsOrFocus;
            Host.AfterGenerateAvailableActionsList += CallAddCancelCritAction;

            Host.Tokens.AssignCondition(new Tokens.DamagedSensorArraySECritToken(Host));
            Triggers.FinishTrigger();
        }

        public override void DiscardEffect()
        {
            Messages.ShowInfo("You can perform actions as usual");
            Host.Tokens.RemoveCondition(typeof(Tokens.DamagedSensorArraySECritToken));

            Host.OnTryAddAvailableAction -= OnlyCancelCritActionsOrFocus;
            Host.AfterGenerateAvailableActionsList -= CallAddCancelCritAction;
        }

        private void OnlyCancelCritActionsOrFocus(GenericAction action, ref bool result)
        {
            if (!action.IsCritCancelAction && !(action is FocusAction))
            {
                result = false;
            }
        }

    }

}