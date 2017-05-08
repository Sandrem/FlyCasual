using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Upgrade
{

    public class GenericSecondaryWeapon : GenericUpgrade
    {
        public int MinRange;
        public int MaxRange;
        public int AttackValue;

        public GenericSecondaryWeapon(Ship.GenericShip host) : base(host)
        {

        }
    }

}
