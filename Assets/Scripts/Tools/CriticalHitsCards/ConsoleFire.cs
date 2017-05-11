using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CriticalHitCard
{

    public class ConsoleFire : GenericCriticalHit
    {
        public ConsoleFire()
        {
            Name = "Console Fire";
            Type = CriticalCardType.Ship;
        }

        public override void ApplyEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("At the start of each Combat phase, roll 1 attack die.");
        }
    }

}