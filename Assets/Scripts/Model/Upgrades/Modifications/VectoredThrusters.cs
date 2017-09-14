using Ship;
using Upgrade;

namespace UpgradesList
{ 
    public class VectoredThrusters : GenericActionBarUpgrade<ActionsList.BarrelRollAction>
    {
        public VectoredThrusters() : base()
        {
            Type = UpgradeType.Modification;
            Name = ShortName = "Vectored Thrusters";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/upgrades/Modification/vectored-thrusters.png";
            Cost = 2;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.ShipBaseSize == BaseSize.Small;         
        }
    }
}
