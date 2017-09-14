using Ship;
using Ship.YT1300;
using Upgrade;

namespace UpgradesList
{ 
    public class MillenniumFalcon : GenericActionBarUpgrade<ActionsList.EvadeAction>
    {
        public MillenniumFalcon() : base()
        {
            Type = UpgradeType.Title;
            Name = ShortName = "Millennium Falcon";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/upgrades/Title/millennium-falcon.png";
            Cost = 1;
            isUnique = true;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is YT1300;
        }
    }
}
