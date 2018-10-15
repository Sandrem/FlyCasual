using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using UnityEngine;

namespace DamageDeckCardSE
{

    public class PanickedPilot : GenericDamageCard
    {
        public PanickedPilot()
        {
            Name = "Panicked Pilot";
            Type = CriticalCardType.Pilot;
            ImageUrl = "https://i.imgur.com/NoxIFIP.jpg";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.Tokens.AssignTokens(CreateStressToken, 2, FinishAndDiscard);
        }

        private GenericToken CreateStressToken()
        {
            return new StressToken(Host);
        }

        private void FinishAndDiscard()
        {
            Triggers.FinishTrigger();
            DiscardEffect();
        }

    }

}