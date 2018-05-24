using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageDeckCardFE
{

    public class DamagedSensorArray : GenericDamageCard
    {
        public DamagedSensorArray()
        {
            Name = "Damaged Sensor Array";
            Type = CriticalCardType.Ship;
            CancelDiceResults.Add(DieSide.Success);
            CancelDiceResults.Add(DieSide.Crit);
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.OnTryAddAvailableAction += OnlyCancelCritActions;
            Host.AfterGenerateAvailableActionsList += CallAddCancelCritAction;

            Host.Tokens.AssignCondition(new Tokens.DamagedSensorArrayCritToken(Host));
            Triggers.FinishTrigger();
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Messages.ShowInfo("You can perform actions as usual");
            Host.Tokens.RemoveCondition(typeof(Tokens.DamagedSensorArrayCritToken));

            Host.OnTryAddAvailableAction -= OnlyCancelCritActions;

            Host.AfterGenerateAvailableActionsList -= CallAddCancelCritAction;
        }

        private void OnlyCancelCritActions(ActionsList.GenericAction action, ref bool result)
        {
            if (!action.IsCritCancelAction)
            {
                result = false;
            }
        }

    }

}