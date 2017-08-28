using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CriticalHitCard
{

    public class DamagedSensorArray : GenericCriticalHit
    {
        public DamagedSensorArray()
        {
            Name = "Damaged Sensor Array";
            Type = CriticalCardType.Ship;
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/damage-decks/core-tfa/damaged-sensor-array.png";
            CancelDiceResults.Add(DiceSide.Success);
            CancelDiceResults.Add(DiceSide.Crit);
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.OnTryAddAvailableAction += OnlyCancelCritActions;
            Host.AfterGenerateAvailableActionsList += AddCancelCritAction;

            Host.AssignToken(new Tokens.DamagedSensorArrayCritToken(), Triggers.FinishTrigger);
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            Messages.ShowInfo("You can perform actions as usual");
            host.RemoveToken(typeof(Tokens.DamagedSensorArrayCritToken));

            host.OnTryAddAvailableAction -= OnlyCancelCritActions;

            host.AfterGenerateAvailableActionsList -= AddCancelCritAction;
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