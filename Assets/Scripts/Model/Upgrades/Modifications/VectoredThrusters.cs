using Ship;
using Upgrade;

namespace UpgradesList
{ 
    public class VectoredThrusters : GenericActionBarUpgrade<ActionsList.BarrelRollAction>
    {
        public VectoredThrusters() : base()
        {
            Type = UpgradeType.Modification;
            Name = "Vectored Thrusters";
            Cost = 2;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.ShipBaseSize == BaseSize.Small;         
        }
    }
}
