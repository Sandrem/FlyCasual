using Editions;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace SquadBuilderNS
{
    public class SquadListShip
    {
        public GenericShip Instance;
        public SquadList List;
        public ShipWithUpgradesPanelInfo Panel;

        public SquadListShip(GenericShip ship, SquadList list)
        {
            List = list;

            InitializeShip(ship);
        }

        private void InitializeShip(GenericShip ship)
        {
            Instance = ship;
            Instance.InitializePilotForSquadBuilder();
        }

        public void InstallUpgrade(UpgradeSlot slot, GenericUpgrade upgrade)
        {
            slot.PreInstallUpgrade(upgrade, SquadBuilder.Instance.CurrentShip.Instance);
        }

        public bool InstallUpgrade(string upgradeNameCanonical, UpgradeType upgradeType)
        {
            string upgradeTypeName = SquadBuilder.Instance.Database.AllUpgrades.Find(n => n.UpgradeNameCanonical == upgradeNameCanonical && n.UpgradeType == upgradeType)?.UpgradeTypeName;

            try
            {
                GenericUpgrade newUpgrade = (GenericUpgrade)System.Activator.CreateInstance(Type.GetType(upgradeTypeName));
                Edition.Current.AdaptUpgradeToRules(newUpgrade);

                return TryInstallUpgade(newUpgrade);
            }
            catch
            {
                if (!string.IsNullOrEmpty(upgradeTypeName)) Messages.ShowError($"Cannot find upgrade: {upgradeTypeName}");
                return false;
            }
        }

        public bool InstallUpgrade(string upgradeTypeName)
        {
            try
            {
                GenericUpgrade newUpgrade = (GenericUpgrade)System.Activator.CreateInstance(Type.GetType(upgradeTypeName));
                Edition.Current.AdaptUpgradeToRules(newUpgrade);

                return TryInstallUpgade(newUpgrade);
            }
            catch
            {
                if (!string.IsNullOrEmpty(upgradeTypeName)) Messages.ShowError($"Cannot find upgrade: {upgradeTypeName}");
                return false;
            }
        }

        public bool TryInstallUpgade(GenericUpgrade upgrade)
        {
            List<UpgradeSlot> slots = FindFreeSlots(upgrade.UpgradeInfo.UpgradeTypes);
            if (slots.Count >= upgrade.UpgradeInfo.UpgradeTypes.Count)
            {
                slots[0].PreInstallUpgrade(upgrade, Instance);
                return true;
            }
            else
            {
                return false;
            }
        }

        private List<UpgradeSlot> FindFreeSlots(List<UpgradeType> upgradeTypes)
        {
            return Instance.UpgradeBar.GetFreeSlots(upgradeTypes);
        }

        public void ClearUpgrades()
        {
            ClearUpgradesRecursive();
        }

        private void ClearUpgradesRecursive()
        {
            UpgradeSlot slot = Instance.UpgradeBar.GetUpgradeSlots().FirstOrDefault(s => !s.IsEmpty);
            if (slot != null)
            {
                slot.RemovePreInstallUpgrade();
                ClearUpgradesRecursive();
            }
        }
    }
}
