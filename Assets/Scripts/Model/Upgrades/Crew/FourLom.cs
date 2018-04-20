using Abilities;
using Ship;
using System;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class FourLom : GenericUpgrade
    {
        public FourLom() : base()
        {
            IsHidden = true;

            Types.Add(UpgradeType.Crew);
            Name = "4-LOM";
            Cost = 1;

            isUnique = true;

            AvatarOffset = new Vector2(30, 1);
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Scum;
        }
    }
}