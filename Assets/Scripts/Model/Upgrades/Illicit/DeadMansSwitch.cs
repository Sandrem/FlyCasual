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
            IsHidden = true;

            Types.Add(UpgradeType.Illicit);
            Name = "Dead Man's Switch";
            Cost = 2;
        }
    }
}
