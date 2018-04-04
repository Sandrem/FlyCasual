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
            slot.Counter = UpgradeSlots.Count(n => n.Type == slot.Type) + 1;
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
            List<UpgradeSlot> freeSlots = GetFreeSlots(upgrade.Types);
            if (freeSlots.Count != upgrade.Types.Count) {
                Debug.Log ("No free slot: " + upgrade.getTypesAsString());
            } else {
                for (int i = 0; i < freeSlots.Count; i++) {
                    UpgradeSlot freeSlot = freeSlots [i];
                    for (int j = 0; j < upgrade.Types.Count; j++) {
                        UpgradeType type = upgrade.Types [j];
                        if (type == freeSlot.Type) {
                            freeSlot.TryInstallUpgrade (upgrade, Host);
                            break;
                        } 
                    }
                }
            }
        }

        public List<UpgradeSlot> GetFreeSlots(List<UpgradeType> upgradeTypes)
        {
            // clone the list in order to search
            List<UpgradeSlot> holder = new List<UpgradeSlot>();
            for (int i = 0; i < UpgradeSlots.Count; i++) {
                holder.Add (UpgradeSlots [i]);
            }

            List<UpgradeSlot> results = new List<UpgradeSlot>();
            for (int i = 0; i < upgradeTypes.Count; i++) {
                UpgradeType uType = upgradeTypes [i];
                for (int j = 0; j < holder.Count; j++) {
                    UpgradeSlot uslot = holder [j];
                    if (uType == uslot.Type && uslot.IsEmpty) {
                        results.Add (uslot);
                        holder.Remove (uslot);
                        break;
                    }
                }
            }
            return results;
        }

        public List<GenericUpgrade> GetUpgradesAll()
        {
            List<GenericUpgrade> result = UpgradeSlots.Where(n => (!n.IsEmpty && n.InstalledUpgrade.GetType() != typeof(UpgradesList.EmptyUpgrade))).ToList().Select(n => n.InstalledUpgrade).ToList();
            if (result == null) result = new List<GenericUpgrade>();
            return result;
        }

        /**
         * Returns a list of installed upgrade based on type.
         * @param type the type of upgrade to return.
         * @return the list of installed upgrade if exists and null otherwise.
         */
        public List<GenericUpgrade> GetInstalledUpgrades(UpgradeType type)
        {
            List<GenericUpgrade> result = new List<GenericUpgrade>();
            for (int i = 0; i < GetUpgradesAll().Count; i++) {
                GenericUpgrade upgrade = GetUpgradesAll() [i];
                for (int j = 0; j < upgrade.Types.Count; j++) {
                    if (upgrade.Types [j] == type) {
                        result.Add (upgrade);
                        break;
                    }
                }
            }
            return result;
        }

        /**
         * Returns the first occurance of an installed upgrade based on type.
         * @param type the type of upgrade to return.
         * @return the installed upgrade if it exists and null otherwise.
         */
        public GenericUpgrade GetInstalledUpgrade(UpgradeType type){
            for (int i = 0; i < GetUpgradesAll ().Count; i++) {
                GenericUpgrade upgrade = GetUpgradesAll() [i];
                for (int j = 0; j < upgrade.Types.Count; j++) {
                    if (upgrade.Types [j] == type) {
                        return upgrade;
                    }
                }
            }
            return null;
        }

        public List<GenericUpgrade> GetUpgradesOnlyFaceup()
        {
            return GetUpgradesAll().Where(n => n.isDiscarded == false).ToList();
        }

        public List<GenericUpgrade> GetUpgradesOnlyDiscarded()
        {
            return GetUpgradesAll().Where(n => n.isDiscarded == true).ToList();
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

        public bool HasUpgradeInstalled(Type upgradeType)
        {
            return GetUpgradesAll().Any(n => n.GetType() == upgradeType);
        }

        /**
         * Checks if the ship has free upgrade slots for the list of upgrade types.
         * @param upgradeTypes the list of upgrades types to check.
         * @return true if there are slots for all upgrade types open and false otherwise.
         */
        public bool HasFreeUpgradeSlot(List<UpgradeType> upgradeTypes)
        {
            // clone the list in order to search
            List<UpgradeSlot> slots = new List<UpgradeSlot> ();
            for (int i = 0; i < UpgradeSlots.Count; i++) {
                slots.Add(UpgradeSlots[i]);
            }

            // the number of slots available
            int count = 0;

            for (int i = 0; i < upgradeTypes.Count; i++) {
                UpgradeType type = upgradeTypes [i];

                for (int j = 0; j < slots.Count; j++) {
                    UpgradeSlot slot = slots [j];
                    if (slot.Type == type && slot.InstalledUpgrade == null) {
                        slots.Remove (slot);
                        count++;
                        break;
                    }
                }
            }
            if (count == upgradeTypes.Count) {
                return true;
            }
            return false;
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
