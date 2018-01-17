using Upgrade;
using UnityEngine;
using Ship;
using System.Collections.Generic;

namespace UpgradesList
{
    public class HotShotBlaster : GenericSecondaryWeapon
    {
        public HotShotBlaster() : base()
        {
            Types.Add(UpgradeType.Illicit);
            Name = "\"Hot Shot\" Blaster";
            Cost = 3;

            MinRange = 1;
            MaxRange = 3;
            AttackValue = 3;

            CanShootOutsideArc = true;

            IsDiscardedForShot = true;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);
        }
    }
}
