using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ship;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class UnguidedRockets : GenericSecondaryWeapon
    {   
        public UnguidedRockets() : base()
        {   
            Types.Add( UpgradeType.Missile);
            Types.Add( UpgradeType.Missile);
            Name        = "Unguided Rockets";
            Cost        = 2;
            MinRange    = 1;
            MaxRange    = 3;
            AttackValue = 3;

            RequiresFocusToShoot = true;
        }   
        // TODO
        // * Do not allow target lock to spend to reroll
        // * Do not allow defender to modifiy attack dice
        // * Only allow focus modification

        public override void AttachToShip(GenericShip host)
        {   
            base.AttachToShip(host);
        }   
    }   
}
