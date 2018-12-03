using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Upgrade
{
    public class UpgradeCardState
    {
        private readonly GenericShip HostShip;
        private readonly GenericUpgrade HostUpgrade;

        public int Charges { get; private set; }
        public int MaxCharges { get; private set; }
        public bool UsesCharges { get { return MaxCharges > 0; } }

        public bool IsFaceup { get; private set; }

        public UpgradeCardState(GenericUpgrade upgrade)
        {
            HostUpgrade = upgrade;
            HostShip = upgrade.Host;

            IsFaceup = true;
            Charges = upgrade.UpgradeInfo.Charges;
            MaxCharges = upgrade.UpgradeInfo.Charges;
        }

        public void Flip(bool isFaceup)
        {
            IsFaceup = isFaceup;
        }

        public void SpendCharges(int count)
        {
            for (int i = 0; i < count; i++)
            {
                SpendCharge();
            }
        }

        public void SpendCharge()
        {
            Charges--;
            if (Charges < 0) throw new InvalidOperationException("Cannot spend charge when you have none left");

            if (Charges == 0) Roster.ShowUpgradeAsInactive(HostShip, HostUpgrade.UpgradeInfo.Name);

            Roster.UpdateUpgradesPanel(HostShip, HostShip.InfoPanel);
        }

        public void RestoreCharge()
        {
            if (Charges < MaxCharges)
            {
                if (Charges == 0) Roster.ShowUpgradeAsActive(HostShip, HostUpgrade.UpgradeInfo.Name);

                Charges++;

                Roster.UpdateUpgradesPanel(HostShip, HostShip.InfoPanel);
            }
        }
    }
}
