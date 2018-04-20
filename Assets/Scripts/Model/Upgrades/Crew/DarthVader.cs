using Abilities;
using Ship;
using System;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class DarthVader : GenericUpgrade
    {
        public DarthVader() : base()
        {
            IsHidden = true;

            Types.Add(UpgradeType.Crew);
            Name = "Darth Vader";
            Cost = 3;

            isUnique = true;

            AvatarOffset = new Vector2(53, 1);
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Imperial;
        }
    }
}