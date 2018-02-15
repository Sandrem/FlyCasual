using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageDeckCard
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
            Host.Tokens.AssignToken(new Tokens.StressToken(Host), Triggers.FinishTrigger);
        }

    }

}