using Upgrade;
using Ship;
using Abilities;
using ActionsList;

namespace UpgradesList
{
    public class TacticalOfficer : GenericUpgrade
    {
        public TacticalOfficer() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Tactical Officer";
            Cost = 2;

            UpgradeAbilities.Add(new GenericActionBarAbility<CoordinateAction>());
            //AvatarOffset = new Vector2(45, 1);
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Imperial;
        }
    }
}