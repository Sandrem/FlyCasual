using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CriticalHitCard
{

    public class StunnedPilot : GenericCriticalHit
    {
        public StunnedPilot()
        {
            Name = "Stunned Pilot";
            Type = CriticalCardType.Pilot;
        }

        public override void ApplyEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("After you execute a maneuver, if you are touching another ship or overlapping an obstacle token, suffer 1 damage.");
        }
    }

}

