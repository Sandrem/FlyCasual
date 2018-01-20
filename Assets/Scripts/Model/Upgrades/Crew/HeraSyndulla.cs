﻿using System;
using Upgrade;

namespace UpgradesList
{
    public class HeraSyndulla : GenericUpgrade
    {
        public HeraSyndulla() : base()
        {
            Type = UpgradeType.Crew;
            Name = "Hera Syndulla";
            Cost = 1;

            isUnique = true;
        }

        public override bool IsAllowedForShip(Ship.GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.CanPerformRedManeuversWhileStressed = true;
        }

        public override void Discard(Action callBack)
        {
            Host.CanPerformRedManeuversWhileStressed = false;

            base.Discard(callBack);
        }
    }
}
