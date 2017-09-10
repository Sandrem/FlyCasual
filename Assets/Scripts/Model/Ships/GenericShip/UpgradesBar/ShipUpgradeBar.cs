using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Upgrade
{
    public class ShipUpgradeBar
    {
        private Ship.GenericShip Host;

        private List<UpgradeSlot> UpgradeSlots;
        private List<UpgradeType> ForbiddenSlots;

        public ShipUpgradeBar(Ship.GenericShip host)
        {
            Host = host;

            UpgradeSlots = new List<UpgradeSlot>();
            ForbiddenSlots = new List<UpgradeType>();

            AddSlot(UpgradeType.Title);
            AddSlot(UpgradeType.Modification);
        }

        public void AddSlot(UpgradeType slotType)
        {
            AddSlot(new UpgradeSlot(slotType));
        }

        public void AddSlot(UpgradeSlot slot)
        {
            UpgradeSlots.Add(slot);
        }

        public void RemoveSlot(UpgradeType upgradeType, object grantedBy = null)
        {
            UpgradeSlot slot = UpgradeSlots.Find(n => (n.Type == upgradeType) && (n.GrantedBy == grantedBy));
            if (slot != null) UpgradeSlots.Remove(slot);
        }

        public void TryInstallUpgrade(string upgradeName)
        {
            GenericUpgrade upgrade = (GenericUpgrade)System.Activator.CreateInstance(System.Type.GetType(upgradeName));
            TryInstallUpgrade(upgrade);
        }

        public void TryInstallUpgrade(GenericUpgrade upgrade)
        {
            UpgradeSlot freeSlot = GetFreeSlot(upgrade.Type);
            if (freeSlot != null)
            {
                freeSlot.TryInstallUpgrade(upgrade, Host);
            }
            else
            {
                Debug.Log("No free slot: " + upgrade.Type);
            }
        }

        public UpgradeSlot GetFreeSlot(UpgradeType upgradeType)
        {
            UpgradeSlot result = null;
            result = UpgradeSlots.Find(n => (n.Type == upgradeType && n.IsEmpty));
            return result;
        }

        public List<GenericUpgrade> GetInstalledUpgrades()
        {
            List<GenericUpgrade> result = new List<GenericUpgrade>();
            result = UpgradeSlots.Where(n => (!n.IsEmpty)).ToList().Select(n => n.InstalledUpgrade).ToList();
            return result;
        }

        public List<UpgradeSlot> GetUpgradeSlots()
        {
            List<UpgradeSlot> result = UpgradeSlots.Where(n => !ForbiddenSlots.Contains(n.Type)).ToList();
            return result;
        }

        public bool HasUpgradeSlot(UpgradeType upgradeType)
        {
            bool result = false;
            result = (UpgradeSlots.Find(n => n.Type == upgradeType) != null);
            return result;
        }

        public int CountUpgradeSlotType(UpgradeType upgradeType)
        {
            return UpgradeSlots.Count(n => n.Type == upgradeType);
        }

        public void ForbidSlots(UpgradeType upgradeType)
        {
            ForbiddenSlots.Add(upgradeType);

            foreach (var slot in UpgradeSlots)
            {
                if (slot.Type == upgradeType)
                {
                    if (slot.InstalledUpgrade != null) slot.RemovePreInstallUpgrade();
                }
            }
        }

        public void AllowSlots(UpgradeType upgradeType)
        {
            if (ForbiddenSlots.Contains(upgradeType)) ForbiddenSlots.Remove(upgradeType);
        }
    }
}
