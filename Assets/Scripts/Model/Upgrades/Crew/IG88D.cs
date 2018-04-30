using Abilities;
using Ship;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class IG88D : GenericUpgrade
    {
        public IG88D() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "IG-88D";
            Cost = 1;

            isUnique = true;

            AvatarOffset = new Vector2(44, 2);

            UpgradeAbilities.Add(new Ig2000Ability());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Scum;
        }
    }
}