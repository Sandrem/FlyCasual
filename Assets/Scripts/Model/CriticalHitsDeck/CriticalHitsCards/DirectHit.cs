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
            ImageUrl = "http://i.imgur.com/W81fPBx.jpg";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Messages.ShowInfo("Additional hull damage");
            Game.UI.AddTestLogEntry("Additional hull damage");
            Host.AssignToken(new Tokens.DirectHitCritToken());

            Host.DecreaseHullValue();

            Triggers.FinishTrigger();
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            host.RemoveToken(typeof(Tokens.DirectHitCritToken));
            if (host.TryRegenHull())
            {
                Messages.ShowInfo("Restored hull point");
                Game.UI.AddTestLogEntry("Restored hull point");
            }
        }
    }

}