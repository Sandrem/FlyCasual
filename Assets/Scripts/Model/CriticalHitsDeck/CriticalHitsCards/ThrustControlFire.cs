using System;
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
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/damage-decks/core-tfa/thrust-control-fire.png";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.AssignToken(new Tokens.StressToken(), Triggers.FinishTrigger);
        }

    }

}