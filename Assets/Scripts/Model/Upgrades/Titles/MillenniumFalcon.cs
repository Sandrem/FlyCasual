using Ship;
using Ship.YT1300;
using Upgrade;

namespace UpgradesList
{ 
    public class MillenniumFalcon : GenericActionBarUpgrade<ActionsList.EvadeAction>
    {
        public MillenniumFalcon() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Millennium Falcon";
            Cost = 1;
            isUnique = true;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is YT1300;
        }
    }
}
