using Abilities;
using Ship;
using System;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class YsanneIsard : GenericUpgrade
    {
        public YsanneIsard() : base()
        {
            IsHidden = true;

            Types.Add(UpgradeType.Crew);
            Name = "Ysanne Isard";
            Cost = 4;

            isUnique = true;

            AvatarOffset = new Vector2(21, 1);
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Imperial;
        }
    }
}