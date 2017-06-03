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
            ImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/6/61/Damaged-sensor-array.png";
            CancelDiceResults.Add(DiceSide.Success);
            CancelDiceResults.Add(DiceSide.Crit);
        }

        public override void ApplyEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("You cannot perform any actions except actions listed on Damage cards.");
            Game.UI.AddTestLogEntry("You cannot perform any actions except actions listed on Damage cards.");
            host.AssignToken(new Tokens.DamagedSensorArrayCritToken());

            host.OnTryPerformAction += OnlyCancelCritActions;

            host.AfterAvailableActionListIsBuilt += AddCancelCritAction;
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("You can perform actions as usual");
            Game.UI.AddTestLogEntry("You can perform actions as usual");
            host.RemoveToken(typeof(Tokens.DamagedSensorArrayCritToken));

            host.OnTryPerformAction -= OnlyCancelCritActions;

            host.AfterAvailableActionListIsBuilt -= AddCancelCritAction;
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