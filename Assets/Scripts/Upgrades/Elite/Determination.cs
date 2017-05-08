using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Upgrade
{

    public class Determination : GenericUpgrade
    {

        public Determination(Ship.GenericShip host) : base(host)
        {
            Type = UpgradeSlot.Elite;
        }

    }

}
