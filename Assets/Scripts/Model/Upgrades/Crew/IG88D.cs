using Abilities;
using Ship;
using Upgrade;

namespace UpgradesList
{
    public class IG88D : GenericUpgrade
    {
        public IG88D() : base()
        {
            Type = UpgradeType.Crew;
            Name = "IG-88D";
            Cost = 1;

            isUnique = true;

            UpgradeAbilities.Add(new Ig2000Ability());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Scum;
        }
    }
}