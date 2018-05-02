using Abilities;
using Ship;
using System;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class Zuckuss : GenericUpgrade
    {
        public Zuckuss() : base()
        {
            IsHidden = true;

            Types.Add(UpgradeType.Crew);
            Name = "Zuckuss";
            Cost = 1;

            isUnique = true;

            AvatarOffset = new Vector2(79, 1);
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Scum;
        }
    }
}