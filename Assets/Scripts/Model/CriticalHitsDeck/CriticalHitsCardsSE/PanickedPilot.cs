using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageDeckCardSE
{

    public class PanickedPilot : GenericDamageCard
    {
        public PanickedPilot()
        {
            Name = "Panicked Pilot";
            Type = CriticalCardType.Pilot;
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.Tokens.AssignToken(new Tokens.StressToken(Host),
                () => Host.Tokens.AssignToken(new Tokens.StressToken(Host),
                () =>
                {
                    Triggers.FinishTrigger();
                    DiscardEffect();                    
                }));
        }

    }

}