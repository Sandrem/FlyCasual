using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CriticalHitCard
{

    public class DirectHit : GenericCriticalHit
    {
        public DirectHit()
        {
            Name = "Direct Hit";
            Type = CriticalCardType.Ship;
        }

        public override void ApplyEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("Additional hull damage");
            Game.UI.AddTestLogEntry("Additional hull damage");

            host.SufferHullDamage();
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            if (host.TryRegenHull())
            {
                Game.UI.ShowInfo("Restored additional hull damage");
                Game.UI.AddTestLogEntry("Restored additional hull damage");
            }
        }
    }

}