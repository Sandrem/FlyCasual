using Abilities;
using Ship;
using System;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class TargetingCoordinator : GenericUpgrade
    {
        public TargetingCoordinator() : base()
        {
            IsHidden = true;

            Types.Add(UpgradeType.Crew);
            Name = "Targeting Coordinator";
            Cost = 4;

            isLimited = true;

            Avatar = new AvatarInfo(Faction.None, new Vector2(39, 1));
        }
    }
}