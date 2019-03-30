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
            Host.OnTryAddAction += OnlyCancelCritActions;
            Host.OnGenerateActions += CallAddCancelCritAction;

            Host.Tokens.AssignCondition(typeof(Tokens.DamagedSensorArrayCritToken));
            Triggers.FinishTrigger();
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Messages.ShowInfo("Damaged Sensor Array has been repaired.  " + Host.PilotInfo.PilotName + " can perform actions as usual.");
            Host.Tokens.RemoveCondition(typeof(Tokens.DamagedSensorArrayCritToken));

            Host.OnTryAddAction -= OnlyCancelCritActions;

            Host.OnGenerateActions -= CallAddCancelCritAction;
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