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
            ImageUrl = "http://i.imgur.com/kUvUwHQ.jpg";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Messages.ShowInfo("Received stress token");
            Game.UI.AddTestLogEntry("Received stress token");

            Host.AssignToken(new Tokens.StressToken(), Triggers.FinishTrigger);
        }

    }

}