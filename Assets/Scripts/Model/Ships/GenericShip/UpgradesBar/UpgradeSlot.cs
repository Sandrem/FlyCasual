using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Upgrade
{
    public class UpgradeSlot
    {
        public UpgradeType Type { get; private set; }
        public object GrantedBy { get; set; }
        public int CostDecrease { get; set; }
        public int MaxCost { get; set; }
        public bool MustBeDifferent { get; set; }
        public GenericUpgrade InstalledUpgrade { get; private set; }

        public int InstalledUpgradeCostReduction { get; private set; }
        public int InstalledUpgradeCostMax { get; private set; }

        public bool IsEmpty
        {
            get { return (InstalledUpgrade == null); }
        }


        public UpgradeSlot(UpgradeType type)
        {
            Type = type;
            CostDecrease = 0;
            MaxCost = int.MaxValue;
        }

        public void TryInstallUpgrade(GenericUpgrade upgrade, Ship.GenericShip host)
        {
            if (CheckRequirements(upgrade))
            {
                InstallUpgrade(upgrade, host);
            }
            else
            {
                Debug.Log("Requirements are not met: " + upgrade.Name);
            }
        }

        public void PreInstallUpgrade(GenericUpgrade upgrade, Ship.GenericShip host)
        {
            InstalledUpgrade = upgrade;
            InstalledUpgrade.PreAttachToShip(host);
        }

        public void RemovePreInstallUpgrade()
        {
            InstalledUpgrade.PreDettachFromShip();
            InstalledUpgrade = null;
        }

        private void InstallUpgrade(GenericUpgrade upgrade, Ship.GenericShip host)
        {
            InstalledUpgrade = upgrade;
            InstalledUpgrade.AttachToShip(host);
        }

        private bool CheckRequirements(GenericUpgrade upgrade)
        {
            bool result = true;
            return result;
        }

    }
}
