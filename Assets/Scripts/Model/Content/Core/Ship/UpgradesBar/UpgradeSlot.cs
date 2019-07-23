using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Upgrade
{
    public class UpgradeSlot
    {
        public GenericShip HostShip { get; private set; }
        public UpgradeType Type { get; private set; }
        public object GrantedBy { get; set; }
        public int Counter { get; set; }

        public int CostDecrease { get; set; }
        public int MaxCost { get; set; }            // Needed for Tie Shuttle Title
        public bool MustBeDifferent { get; set; }   // Needed fot Royal Guard Title
        public bool MustBeUnique { get; set; }   // Needed for Havoc Title

        public GenericUpgrade InstalledUpgrade { get; private set; }

        public int InstalledUpgradeCostReduction { get; private set; }
        public int InstalledUpgradeCostMax { get; private set; }

        public event Ship.GenericShip.EventHandlerUpgrade OnPreInstallUpgrade;
        public event Ship.GenericShip.EventHandlerUpgrade OnRemovePreInstallUpgrade;

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

        public void TryInstallUpgrade(GenericUpgrade upgrade, GenericShip host)
        {
            HostShip = host;
            if (upgrade != null) InstallUpgrade();
        }

        public void PreInstallUpgrade(GenericUpgrade upgrade, GenericShip host)
        {
            HostShip = host;
            InstalledUpgrade = upgrade;
            upgrade.Slot = this;
            InstalledUpgrade.PreAttachToShip(host);

            if (upgrade.UpgradeInfo.UpgradeTypes.Count > 1)
            {
                List<UpgradeType> extraSlots = new List<UpgradeType>(upgrade.UpgradeInfo.UpgradeTypes);
                extraSlots.Remove(extraSlots.First(n => n == this.Type));

                foreach (UpgradeType upgradeType in extraSlots)
                {
                    // Clone upgrade to fill extra slots
                    UpgradesList.EmptyUpgrade emptyUpgrade = new UpgradesList.EmptyUpgrade();
                    emptyUpgrade.SetUpgradeInfo(upgradeType, upgrade.UpgradeInfo.Name, 0);

                    UpgradeSlot tempSlot = host.UpgradeBar.GetUpgradeSlots().First(n => n.IsEmpty && emptyUpgrade.HasType(n.Type));
                    tempSlot.PreInstallUpgrade(emptyUpgrade, host);
                }
            }

            if (DebugManager.FreeMode)
            {
                if (!host.UpgradeBar.HasFreeUpgradeSlot(new List<UpgradeType>(){ UpgradeType.Omni })) host.UpgradeBar.AddSlot(UpgradeType.Omni);
            }

            OnPreInstallUpgrade?.Invoke(upgrade);
        }

        public void RemovePreInstallUpgrade()
        {
            InstalledUpgrade.PreDettachFromShip();
            OnRemovePreInstallUpgrade?.Invoke(InstalledUpgrade);
            InstalledUpgrade = null;

            if (DebugManager.FreeMode)
            {
                if (HostShip.UpgradeBar.GetUpgradeSlots().Count(n => n.Type == UpgradeType.Omni && n.IsEmpty) > 1)
                {
                    HostShip.UpgradeBar.RemoveEmptySlot(UpgradeType.Omni);
                }
            }
        }

        private void InstallUpgrade()
        {
            //TODO: Remove host paramater
            InstalledUpgrade.AttachToShip(InstalledUpgrade.HostShip);
        }
    }
}
