using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{

    public partial class GenericShip
    {
        public Dictionary<Upgrade.UpgradeType, int> BuiltInSlots { get; protected set; }
        public List<KeyValuePair<Upgrade.UpgradeType, Upgrade.GenericUpgrade>> InstalledUpgrades { get; protected set; }

        //UPGRADES

        private void AddCoreUpgradeSlots()
        {
            AddUpgradeSlot(Upgrade.UpgradeType.Modification);
        }

        protected void AddUpgradeSlot(Upgrade.UpgradeType slot)
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

            Upgrade.UpgradeType slot = newUpgrade.Type;
            if (HasFreeUpgradeSlot(slot))
            {
                newUpgrade.AttachToShip(this);
                InstalledUpgrades.Add(new KeyValuePair<Upgrade.UpgradeType, Upgrade.GenericUpgrade>(newUpgrade.Type, newUpgrade));
                //Roster.UpdateUpgradesPanel(this, this.InfoPanel);
                Roster.OrganizeRosters();
            }
        }

        private bool HasFreeUpgradeSlot(Upgrade.UpgradeType slot)
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
