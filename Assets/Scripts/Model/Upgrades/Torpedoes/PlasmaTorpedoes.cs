using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class PlasmaTorpedoes : GenericSecondaryWeapon
    {
        public PlasmaTorpedoes() : base()
        {
            IsHidden = true;

            Types.Add(UpgradeType.Torpedo);

            Name = "Plasma Torpedoes";
            Cost = 3;

            MinRange = 2;
            MaxRange = 3;
            AttackValue = 4;

            RequiresTargetLockOnTargetToShoot = true;
            
            SpendsTargetLockOnTargetToShoot = true;
            IsDiscardedForShot = true;
        }
    }
}

