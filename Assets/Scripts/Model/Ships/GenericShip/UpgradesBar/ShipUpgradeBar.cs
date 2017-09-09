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

        public void AddSlot(UpgradeType type, object grantedBy = null)
        {
            UpgradeSlot slot = new UpgradeSlot(type, grantedBy);
            UpgradeSlots.Add(slot);
        }

        public void RemoveSlot(System.Type type, object grantedBy = null)
        {
            UpgradeSlot slot = UpgradeSlots.Find(n => (n.GetType() == type) && (n.GrantedBy == grantedBy));
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
            return UpgradeSlots;
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
    }
}
