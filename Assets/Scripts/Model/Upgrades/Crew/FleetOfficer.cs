using Abilities;
using Ship;
using System;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class FleetOfficer : GenericUpgrade
    {
        public FleetOfficer() : base()
        {
            IsHidden = true;

            Types.Add(UpgradeType.Crew);
            Name = "Fleet Officer";
            Cost = 3;

            AvatarOffset = new Vector2(19, 1);
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Imperial;
        }
    }
}