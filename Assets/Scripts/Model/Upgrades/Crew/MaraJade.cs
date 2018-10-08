using Abilities;
using Ship;
using System;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class MaraJade : GenericUpgrade
    {
        public MaraJade() : base()
        {
            IsHidden = true;

            Types.Add(UpgradeType.Crew);
            Name = "Mara Jade";
            Cost = 3;

            isUnique = true;

            Avatar = new AvatarInfo(Faction.Imperial, new Vector2(39, 1));
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Imperial;
        }
    }
}