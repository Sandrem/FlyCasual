using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ship
{
    public class UpgradeSlot
    {
        public Upgrade.UpgradeType Type { get; private set; }
        public Upgrade.GenericUpgrade InstalledUpgrade { get; private set; }

        public int InstalledUpgradeCostReduction { get; private set; }
        public int InstalledUpgradeCostMax { get; private set; }

        public UpgradeSlot(Upgrade.UpgradeType type)
        {
            Type = type;
        }

        public void InstallUpgrade(Upgrade.GenericUpgrade upgrade)
        {
            InstalledUpgrade = upgrade;
        }
    }

    public class UpgradesBar
    {
        public List<UpgradeSlot> PrintedUpgradeSlots { get; private set; }
        public List<UpgradeSlot> UpgradeSlots { get; private set; }

        public List<Upgrade.UpgradeType> ForbiddenSlots { get; private set; }

        public UpgradesBar()
        {
            PrintedUpgradeSlots = new List<UpgradeSlot>();
            UpgradeSlots = new List<UpgradeSlot>();
            ForbiddenSlots = new List<Upgrade.UpgradeType>();
        }

        public void AddSlot(Upgrade.UpgradeType type, bool isPrinted = false)
        {
            UpgradeSlot slot = new UpgradeSlot(type);
            UpgradeSlots.Add(slot);
            if (isPrinted) PrintedUpgradeSlots.Add(slot);
        }

        public void RemoveSlot(System.Type type)
        {
            UpgradeSlot slot = UpgradeSlots.Find(n => n.GetType() == type);
            if (slot != null) UpgradeSlots.Remove(slot);
        }
    }
}
