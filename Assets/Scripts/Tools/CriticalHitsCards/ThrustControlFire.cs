using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CriticalHitCard
{

    public class ThrustControlFire : GenericCriticalHit
    {
        public ThrustControlFire()
        {
            Name = "Thrust Control Fire";
            Type = CriticalCardType.Ship;
        }

        public override void ApplyEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("Received stress token");
            Game.UI.AddTestLogEntry("Received stress token");

            host.AssignToken(new Tokens.StressToken());
        }

    }

}