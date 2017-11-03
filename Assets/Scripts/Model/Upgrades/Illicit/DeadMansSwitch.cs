using Upgrade;
using UnityEngine;
using Ship;
using System.Collections.Generic;

namespace UpgradesList
{
    public class DeadMansSwitch : GenericUpgrade
    {
        public DeadMansSwitch() : base()
        {
            Type = UpgradeType.Illicit;
            Name = "Dead Man's Switch";
            Cost = 2;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);
        }
    }
}
