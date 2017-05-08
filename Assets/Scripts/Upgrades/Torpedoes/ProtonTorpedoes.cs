using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Upgrade
{

    public class ProtonTorpedoes : GenericSecondaryWeapon
    {
        public ProtonTorpedoes(Ship.GenericShip host) : base(host)
        {
            Type = UpgradeSlot.Torpedoes;
            MinRange = 2;
            MaxRange = 3;
            AttackValue = 4;
        }
    }

}
