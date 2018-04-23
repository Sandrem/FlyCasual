using Abilities;
using Ship;
using System;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class GrandMoffTarkin : GenericUpgrade
    {
        public GrandMoffTarkin() : base()
        {
            IsHidden = true;

            Types.Add(UpgradeType.Crew);
            Name = "Grand Moff Tarkin";
            Cost = 6;

            isUnique = true;

            AvatarOffset = new Vector2(68, 1);
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Imperial;
        }
    }
}