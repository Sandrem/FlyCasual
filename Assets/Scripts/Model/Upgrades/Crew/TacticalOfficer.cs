using Upgrade;
using Ship;
using Abilities;
using ActionsList;
using System.Linq;
using RuleSets;

namespace UpgradesList
{
    public class TacticalOfficer : GenericUpgrade, ISecondEditionUpgrade
    {
        private bool isSecondEdition = false;

        public TacticalOfficer() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Tactical Officer";
            Cost = 2;

            UpgradeAbilities.Add(new GenericActionBarAbility<CoordinateAction>());
            //AvatarOffset = new Vector2(45, 1);
        }

        public void AdaptUpgradeToSecondEdition()
        {
            ImageUrl = "https://imgur.com/a/ytRDiwM";
            isSecondEdition = true;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            if (isSecondEdition) return ship.PrintedActions.Any(a => a is CoordinateAction && (a as CoordinateAction).IsRed);
            else return ship.faction == Faction.Imperial;
        }
    }
}