using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CriticalHitCard
{
    public class BlindedPilot : GenericCriticalHit
    {
        public BlindedPilot()
        {
            Name = "Blinded Pilot";
            Type = CriticalCardType.Pilot;
        }

        public override void ApplyEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("Cannot perform attack next time");
        }
    }

}