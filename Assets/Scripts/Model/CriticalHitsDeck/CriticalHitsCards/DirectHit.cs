using System;
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
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/damage-decks/core-tfa/direct-hit.png";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.AssignToken(new Tokens.DirectHitCritToken(), AdditionalHullDamage);
        }

        private void AdditionalHullDamage()
        {
            Host.DecreaseHullValue(Triggers.FinishTrigger);
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            host.RemoveToken(typeof(Tokens.DirectHitCritToken));
            if (host.TryRegenHull())
            {
                Messages.ShowInfo("One hull point is restored");
            }
        }
    }

}