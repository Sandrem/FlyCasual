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

        public void TryInstallUpgrade(GenericUpgrade upgrade, Ship.GenericShip host)
        {
            if (upgrade != null) InstallUpgrade();

            /*if (CheckRequirements(upgrade))
            {
                InstallUpgrade(upgrade, host);
            }
            else
            {
                Debug.Log("Requirements are not met: " + upgrade.Name);
            }*/
        }

        public void PreInstallUpgrade(GenericUpgrade upgrade, Ship.GenericShip host)
        {
            InstalledUpgrade = upgrade;
            upgrade.Slot = this;
            InstalledUpgrade.PreAttachToShip(host);

            // check if its a dual upgrade
            if (upgrade.UpgradeInfo.UpgradeTypes.Count > 1)
            {
                // clone upgrade
                //GenericUpgrade newUpgrade = (GenericUpgrade)System.Activator.CreateInstance(upgrade.Types[0]);
                UpgradesList.EmptyUpgrade emptyUpgrade = new UpgradesList.EmptyUpgrade();
                emptyUpgrade.set(upgrade.UpgradeInfo.UpgradeTypes, upgrade.Name, 0);

                int emptySlotsFilled = 0; // Fixes bug #708. TODO: Will need to revisit to support multi-type upgrades.
                // find another slot
                foreach (UpgradeSlot tempSlot in host.UpgradeBar.GetUpgradeSlots())
                {
                    if (emptySlotsFilled < emptyUpgrade.UpgradeInfo.UpgradeTypes.Count && tempSlot.IsEmpty && upgrade.HasType(tempSlot.Type))
                    {
                        emptySlotsFilled += 1; // Fixes bug #708.
                        tempSlot.PreInstallUpgrade(emptyUpgrade, host);
                    }
                }
            }

            if (OnPreInstallUpgrade != null) OnPreInstallUpgrade(upgrade);
        }

        public void RemovePreInstallUpgrade()
        {
            InstalledUpgrade.PreDettachFromShip();
            if (OnRemovePreInstallUpgrade != null) OnRemovePreInstallUpgrade(InstalledUpgrade);
            InstalledUpgrade = null;
        }

        private void InstallUpgrade()
        {
            //TODO: Remove host paramater
            InstalledUpgrade.AttachToShip(InstalledUpgrade.Host);
        }

        //No more used ?
        /*private bool CheckRequirements(GenericUpgrade upgrade)
        {
            bool result = true;
            return result;
        }
        
        public bool UpgradeIsAllowed(GenericUpgrade upgrade)
        {
            bool result = true;

            if (upgrade.Cost > MaxCost)
            {
                //Messages.ShowError(upgrade.Name + "cannot be installed. Max cost: " + MaxCost);
                result = false;
            }

            if (MustBeNonUnique && upgrade.isUnique)
            {
                result = false;
            }

            return result;
        }*/

    }
}
