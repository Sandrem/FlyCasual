using Abilities;
using Ship;
using System;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class Gonk : GenericUpgrade
    {
        public Gonk() : base()
        {
            IsHidden = true;

            Types.Add(UpgradeType.Crew);
            Name = "Gonk";
            Cost = 2;

            isUnique = true;

            AvatarOffset = new Vector2(20, 1);
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Scum;
        }
    }
}