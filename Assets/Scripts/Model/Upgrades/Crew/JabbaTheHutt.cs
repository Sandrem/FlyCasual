using Abilities;
using Ship;
using System;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class JabbaTheHutt : GenericUpgrade
    {
        public JabbaTheHutt() : base()
        {
            IsHidden = true;

            Types.Add(UpgradeType.Crew);
            Types.Add(UpgradeType.Crew);
            Name = "Jabba the Hutt";
            Cost = 5;

            isUnique = true;

            AvatarOffset = new Vector2(58, 5);
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Scum;
        }
    }
}