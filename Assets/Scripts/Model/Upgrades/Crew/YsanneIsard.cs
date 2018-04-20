using Abilities;
using Ship;
using System;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class RearAdmiralChiraneau : GenericUpgrade
    {
        public RearAdmiralChiraneau() : base()
        {
            IsHidden = true;

            Types.Add(UpgradeType.Crew);
            Name = "Rear Admiral Chiraneau";
            Cost = 3;

            isUnique = true;

            AvatarOffset = new Vector2(67, 1);
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Imperial;
        }
    }
}