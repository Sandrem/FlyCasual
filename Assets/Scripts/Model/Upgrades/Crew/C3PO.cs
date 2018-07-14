using Abilities;
using Ship;
using System;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class C3PO : GenericUpgrade
    {
        public C3PO() : base()
        {
            IsHidden = true;

            Types.Add(UpgradeType.Crew);
            Name = "C-3PO";
            Cost = 3;

            isUnique = true;

            AvatarOffset = new Vector2(47, 1);
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }
    }
}