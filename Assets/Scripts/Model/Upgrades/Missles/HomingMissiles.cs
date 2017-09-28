using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class HomingMissiles : GenericSecondaryWeapon
    {
        public HomingMissiles() : base()
        {
            Type = UpgradeType.Missile;

            Name = "Homing Missiles";
            Cost = 5;

            MinRange = 2;
            MaxRange = 3;
            AttackValue = 4;

            RequiresTargetLockOnTargetToShoot = true;

            IsDiscardedForShot = true;
        }
    }
}