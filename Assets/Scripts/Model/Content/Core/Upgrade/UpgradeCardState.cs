using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Upgrade
{
    public class UpgradeCardState
    {
        public int Charges { get; private set; }
        public int MaxCharges { get; private set; }
        public bool IsFaceup { get; private set; }

        public UpgradeCardState(GenericUpgrade upgrade)
        {
            IsFaceup = true;
            Charges = upgrade.Charges;
            MaxCharges = upgrade.Charges;
        }

        public void Flip(bool isFaceup)
        {
            IsFaceup = isFaceup;
        }
    }
}
