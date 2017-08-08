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
            ImageUrl = "http://i.imgur.com/dRl6GLL.jpg";
            CancelDiceResults.Add(DiceSide.Success);
            CancelDiceResults.Add(DiceSide.Crit);
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Game.UI.ShowInfo("You cannot perform any actions except actions listed on Damage cards.");
            Game.UI.AddTestLogEntry("You cannot perform any actions except actions listed on Damage cards.");
            Host.AssignToken(new Tokens.DamagedSensorArrayCritToken());

            Host.OnTryAddAvailableAction += OnlyCancelCritActions;

            Host.AfterGenerateAvailableActionsList += AddCancelCritAction;
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("You can perform actions as usual");
            Game.UI.AddTestLogEntry("You can perform actions as usual");
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