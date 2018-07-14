using Abilities;
using ActionsList;
using Ship;
using Upgrade;

namespace UpgradesList
{ 
    public class VectoredThrusters : GenericUpgrade
    {
        public VectoredThrusters() : base()
        {
            Types.Add(UpgradeType.Modification);
            Name = "Vectored Thrusters";
            Cost = 2;

            UpgradeAbilities.Add(new GenericActionBarAbility<BarrelRollAction>());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.ShipBaseSize == BaseSize.Small;         
        }
    }
}
