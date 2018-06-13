using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using UnityEngine;

namespace DamageDeckCardFE
{

    public class ThrustControlFire : GenericDamageCard
    {
        public ThrustControlFire()
        {
            Name = "Thrust Control Fire";
            Type = CriticalCardType.Ship;
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.Tokens.AssignToken(typeof(StressToken), Triggers.FinishTrigger);
        }

    }

}