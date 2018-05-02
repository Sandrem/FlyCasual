using System;
using UnityEngine;
using Upgrade;
using Abilities;

namespace UpgradesList
{
    public class HeraSyndulla : GenericUpgrade
    {
        public HeraSyndulla() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Hera Syndulla";
            Cost = 1;

            isUnique = true;

            AvatarOffset = new Vector2(36, 1);

            UpgradeAbilities.Add(new HeraSyndullaCrewAbility());
        }

        public override bool IsAllowedForShip(Ship.GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }
    }
}

namespace Abilities
{
    public class HeraSyndullaCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.CanPerformRedManeuversWhileStressed = true;
        }

        public override void DeactivateAbility()
        {
            HostShip.CanPerformRedManeuversWhileStressed = false;
        }
    }
}
