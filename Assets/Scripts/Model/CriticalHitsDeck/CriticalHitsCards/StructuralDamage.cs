using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CriticalHitCard
{

    public class StructuralDamage : GenericCriticalHit
    {
        public StructuralDamage()
        {
            Name = "Structural Damage";
            Type = CriticalCardType.Ship;
        }

        public override void ApplyEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("Agility is reduced by 1");
            Game.UI.AddTestLogEntry("Agility is reduced by 1");

            host.AfterGetAgility += ReduceAgility;
            Roster.UpdateShipStats(host);
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("Agility is restored");
            Game.UI.AddTestLogEntry("Agility is restored");

            host.AfterGetAgility -= ReduceAgility;
            Roster.UpdateShipStats(host);
        }

        private void ReduceAgility(ref int value)
        {
            value--;
        }

    }

}