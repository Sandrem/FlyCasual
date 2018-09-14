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
            SEImageNumber = 48;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            if (isSecondEdition) return ship.ActionBar.HasAction(typeof(CoordinateAction), isRed:true);
            else return ship.faction == Faction.Imperial;
        }
    }
}