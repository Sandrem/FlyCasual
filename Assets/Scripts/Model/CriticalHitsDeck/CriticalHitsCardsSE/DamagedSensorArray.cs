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
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.OnTryAddAvailableAction += OnlyCancelCritActionsOrFocus;
            Host.AfterGenerateAvailableActionsList += CallAddCancelCritAction;

            Host.Tokens.AssignCondition(new Tokens.DamagedSensorArrayCritToken(Host));
            Triggers.FinishTrigger();
        }

        public override void DiscardEffect()
        {
            Messages.ShowInfo("You can perform actions as usual");
            Host.Tokens.RemoveCondition(typeof(Tokens.DamagedSensorArrayCritToken));

            Host.OnTryAddAvailableAction -= OnlyCancelCritActionsOrFocus;
            Host.AfterGenerateAvailableActionsList -= CallAddCancelCritAction;
        }

        private void OnlyCancelCritActionsOrFocus(ActionsList.GenericAction action, ref bool result)
        {
            if (!action.IsCritCancelAction && !(action is FocusAction))
            {
                result = false;
            }
        }

    }

}