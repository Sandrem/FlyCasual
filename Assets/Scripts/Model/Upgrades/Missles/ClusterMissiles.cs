using RuleSets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class ClusterMissiles : GenericSecondaryWeapon, ISecondEditionUpgrade
    {
        public ClusterMissiles() : base()
        {
            Types.Add(UpgradeType.Missile);

            Name = "Cluster Missiles";
            Cost = 4;

            MinRange = 1;
            MaxRange = 2;
            AttackValue = 3;

            RequiresTargetLockOnTargetToShoot = true;

            SpendsTargetLockOnTargetToShoot = true;
            IsDiscardedForShot = true;

            IsTwinAttack = true;
        }

        public void AdaptUpgradeToSecondEdition()
        {
            MaxCharges = 3;
            Cost = 10;

            ImageUrl = "https://i.imgur.com/O7AIHt3.png";
        }
    }

}