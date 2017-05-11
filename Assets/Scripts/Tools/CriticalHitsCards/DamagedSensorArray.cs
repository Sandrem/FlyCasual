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
        }

        public override void ApplyEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("You cannot perform any actions except actions listed on Damage cards.");
        }
    }

}