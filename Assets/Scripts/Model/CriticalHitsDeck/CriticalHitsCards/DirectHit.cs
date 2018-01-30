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
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.Tokens.AssignCondition(new Tokens.DirectHitCritToken(Host));
            AdditionalHullDamage();
        }

        private void AdditionalHullDamage()
        {
            Host.DecreaseHullValue(Triggers.FinishTrigger);
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            host.Tokens.RemoveCondition(typeof(Tokens.DirectHitCritToken));
            if (host.TryRegenHull())
            {
                Messages.ShowInfo("One hull point is restored");
            }
        }
    }

}