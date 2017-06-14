using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{

    public partial class GenericShip
    {
        public Dictionary<Upgrade.UpgradeSlot, int> BuiltInSlots = new Dictionary<Upgrade.UpgradeSlot, int>();
        public List<KeyValuePair<Upgrade.UpgradeSlot, Upgrade.GenericUpgrade>> InstalledUpgrades = new List<KeyValuePair<Upgrade.UpgradeSlot, Upgrade.GenericUpgrade>>();

        //UPGRADES

        private void AddCoreUpgradeSlots()
        {
            AddUpgradeSlot(Upgrade.UpgradeSlot.Modification);
        }

        protected void AddUpgradeSlot(Upgrade.UpgradeSlot slot)
        {
            if (!BuiltInSlots.ContainsKey(slot))
            {
                BuiltInSlots.Add(slot, 1);
            }
            else
            {
                BuiltInSlots[slot]++;
            }

        }

        public void InstallUpgrade(string upgradeName)
        {
            Upgrade.GenericUpgrade newUpgrade = (Upgrade.GenericUpgrade)System.Activator.CreateInstance(System.Type.GetType(upgradeName));

            Upgrade.UpgradeSlot slot = newUpgrade.Type;
            if (HasFreeUpgradeSlot(slot))
            {
                newUpgrade.AttachToShip(this);
                InstalledUpgrades.Add(new KeyValuePair<Upgrade.UpgradeSlot, Upgrade.GenericUpgrade>(newUpgrade.Type, newUpgrade));
                Roster.UpdateUpgradesPanel(this, this.InfoPanel);
                Roster.OrganizeRosters();
            }
        }

        private bool HasFreeUpgradeSlot(Upgrade.UpgradeSlot slot)
        {
            bool result = false;
            if (BuiltInSlots.ContainsKey(slot))
            {
                int slotsAvailabe = BuiltInSlots[slot];
                foreach (var upgrade in InstalledUpgrades)
                {
                    if (upgrade.Key == slot) slotsAvailabe--;
                }

                if (slotsAvailabe > 0) result = true;
            }
            return result;
        }

    }

}
