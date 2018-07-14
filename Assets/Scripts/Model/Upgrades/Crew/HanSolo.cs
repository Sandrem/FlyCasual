using Abilities;
using Ship;
using System;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class HanSolo : GenericUpgrade
    {
        public HanSolo() : base()
        {
            IsHidden = true;

            Types.Add(UpgradeType.Crew);
            Name = "Han Solo";
            Cost = 2;

            isUnique = true;

            AvatarOffset = new Vector2(88, 2);
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }
    }
}