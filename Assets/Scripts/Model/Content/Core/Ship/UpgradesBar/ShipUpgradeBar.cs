using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UpgradesList;

namespace Upgrade
{
    public class ShipUpgradeBar
    {
        private GenericShip HostShip;

        private List<UpgradeSlot> UpgradeSlots;
        private List<UpgradeType> ForbiddenSlots;
        private Dictionary<UpgradeType, int> CostReductionByType;

        public List<GenericUpgrade> InstalledUpgradesAll_System { get; set; }

        public ShipUpgradeBar(GenericShip hostShip)
        {
            HostShip = hostShip;

            UpgradeSlots = new List<UpgradeSlot>();
            ForbiddenSlots = new List<UpgradeType>();
            CostReductionByType = new Dictionary<UpgradeType, int>();
            InstalledUpgradesAll_System = new List<GenericUpgrade>();
        }

        public void AddSlot(UpgradeType slotType)
        {
            UpgradeSlot slot = new UpgradeSlot(slotType);
            slot.OnPreInstallUpgrade += HostShip.CallOnPreInstallUpgrade;
            slot.OnRemovePreInstallUpgrade += HostShip.CallOnRemovePreInstallUpgrade;
            AddSlot(slot);
        }

        public void AddSlot(UpgradeSlot slot)
        {
            slot.Counter = UpgradeSlots.Count(n => n.Type == slot.Type) + 1;
            UpgradeSlots.Add(slot);
        }

        public void RemoveSlot(UpgradeType upgradeType, object grantedBy = null)
        {
            UpgradeSlot slot = UpgradeSlots.Find(n => (n.Type == upgradeType) && (n.GrantedBy == grantedBy));
            if (slot != null)
            {
                if (slot.InstalledUpgrade != null)
                {
                    if (slot.InstalledUpgrade is EmptyUpgrade)
                    {
                        UpgradeSlot realUpgradeSlot = UpgradeSlots.Find(n => (n.InstalledUpgrade != null) && (n.InstalledUpgrade.UpgradeInfo.Name == slot.InstalledUpgrade.UpgradeInfo.Name) && (!(n.InstalledUpgrade is EmptyUpgrade)));
                        realUpgradeSlot.RemovePreInstallUpgrade();
                    }
                    else
                    {
                        slot.RemovePreInstallUpgrade();
                    }
                }
                UpgradeSlots.Remove(slot);
            }
        }

        public void RemoveEmptySlot(UpgradeType upgradeType, object grantedBy = null)
        {
            UpgradeSlot slot = UpgradeSlots.Find(n => (n.Type == upgradeType) && (n.GrantedBy == grantedBy) && n.IsEmpty);
            if (slot != null) UpgradeSlots.Remove(slot);
        }

        public void TryInstallUpgrade(string upgradeName)
        {
            GenericUpgrade upgrade = (GenericUpgrade)System.Activator.CreateInstance(System.Type.GetType(upgradeName));
            TryInstallUpgrade(upgrade);
        }

        public void TryInstallUpgrade(GenericUpgrade upgrade)
        {
            List<UpgradeSlot> freeSlots = GetFreeSlots(upgrade.UpgradeInfo.UpgradeTypes);
            if (freeSlots.Count != upgrade.UpgradeInfo.UpgradeTypes.Count) {
                Debug.Log ("No free slot: " + upgrade.getTypesAsString());
            } else {
                for (int i = 0; i < freeSlots.Count; i++) {
                    UpgradeSlot freeSlot = freeSlots [i];
                    for (int j = 0; j < upgrade.UpgradeInfo.UpgradeTypes.Count; j++) {
                        UpgradeType type = upgrade.UpgradeInfo.UpgradeTypes[j];
                        if (type == freeSlot.Type) {
                            freeSlot.TryInstallUpgrade (upgrade, HostShip);
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
                    if ((uType == uslot.Type || uslot.Type == UpgradeType.Omni) && uslot.IsEmpty) {
                        results.Add (uslot);
                        holder.Remove (uslot);
                        break;
                    }
                }
            }
            return results;
        }

        public bool HasFreeSlots()
        {
            return UpgradeSlots.Where(n => n.InstalledUpgrade == null).ToList().Count > 0;
        }

        public List<GenericUpgrade> GetUpgradesAll()
        {
            List<GenericUpgrade> result = new List<GenericUpgrade>();
            result.AddRange(InstalledUpgradesAll_System.Where(n => !n.isPlaceholder));
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
                for (int j = 0; j < upgrade.UpgradeInfo.UpgradeTypes.Count; j++) {
                    if (upgrade.UpgradeInfo.UpgradeTypes[j] == type) {
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
                for (int j = 0; j < upgrade.UpgradeInfo.UpgradeTypes.Count; j++) {
                    if (upgrade.UpgradeInfo.UpgradeTypes[j] == type) {
                        return upgrade;
                    }
                }
            }
            return null;
        }

        public List<GenericUpgrade> GetUpgradesOnlyFaceup()
        {
            return GetUpgradesAll().Where(n => n.State.IsFaceup).ToList();
        }

        public List<GenericUpgrade> GetSpecialWeaponsAll()
        {
            return GetUpgradesAll().Where(n => n is GenericSpecialWeapon).ToList();
        }

        public List<GenericUpgrade> GetSpecialWeaponsActive()
        {
            return GetUpgradesAll().Where(n =>
                n is GenericSpecialWeapon
                && n.State.IsFaceup
                && (
                    ((n as GenericSpecialWeapon).WeaponInfo.UsesCharges == false)
                    || ((n as GenericSpecialWeapon).WeaponInfo.UsesCharges && n.State.Charges > 0)
                )
            ).ToList();
        }

        public List<GenericUpgrade> GetUpgradesOnlyDiscarded()
        {
            return GetUpgradesAll().Where(n => n.State.IsFaceup == false).ToList();
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

        public bool HasUpgradeTypeInstalled(UpgradeType upgradeType)
        {
            return GetUpgradesAll().Any(n => n.HasType(upgradeType));
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

        public void CostReduceByType(UpgradeType upgradeType, int costReduction)
        {
            if (!CostReductionByType.ContainsKey(upgradeType)) CostReductionByType.Add(upgradeType, costReduction);

            foreach (var slot in UpgradeSlots)
            {
                if (slot.Type == upgradeType)
                {
                    slot.CostDecrease += costReduction;
                }
            }
        }


        public List<GenericUpgrade> GetRechargableUpgrades(UpgradeType type)
        {
            return GetRechargableUpgrades(new List<UpgradeType> { type });
        }

        public List<GenericUpgrade> GetRechargableUpgrades(List<UpgradeType> types)
        {
            return GetUpgradesOnlyFaceup()
                .Where(n => types.Any(type => n.UpgradeInfo.HasType(type)))
                .Where(n => n.State.UsesCharges)
                .Where(n => n.State.Charges < n.State.MaxCharges)
                .Where(n => !n.UpgradeInfo.CannotBeRecharged)
                .ToList();
        }

        public int GetTotalUsedLoadoutCost()
        {
            return GetUpgradeSlots()
                .Where(n => !n.IsEmpty)
                .Sum(n => n.InstalledUpgrade.UpgradeInfo.Cost);
        }
    }
}
