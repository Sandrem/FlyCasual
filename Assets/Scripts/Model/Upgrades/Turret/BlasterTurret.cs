using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList
{

    public class BlasterTurret : GenericSecondaryWeapon
    {
        public BlasterTurret() : base()
        {
            Type = UpgradeType.Turret;

            Name = "Blaster Turret";
            Cost = 4;

            MinRange = 1;
            MaxRange = 2;
            AttackValue = 3;

            CanShootOutsideArc = true;

            RequiresFocusToShoot = true;
            SpendsFocusToShoot = true;
        }
    }
}
