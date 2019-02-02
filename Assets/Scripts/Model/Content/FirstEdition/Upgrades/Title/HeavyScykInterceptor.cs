using Ship;
using Upgrade;
using System.Collections.Generic;
using System.Linq;

namespace UpgradesList.FirstEdition
{
    public class HeavyScykInterceptor : GenericUpgrade
    {
        public HeavyScykInterceptor() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "\"Heavy Scyk\" Interceptor",
                UpgradeType.Title,
                cost: 2,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.M3AInterceptor.M3AInterceptor)),
                addHull: 1,
                abilityType: typeof(Abilities.FirstEdition.HardPointAbility)
            );
        }
    }
}


namespace Abilities.FirstEdition
{
    public class HardPointAbility : GenericAbility
    {
        private readonly List<UpgradeType> HardpointSlotTypes = new List<UpgradeType>
        {
            UpgradeType.Cannon,
            UpgradeType.Torpedo,
            UpgradeType.Missile
        };

        public override void ActivateAbilityForSquadBuilder()
        {
            foreach (UpgradeType upgradeType in HardpointSlotTypes)
            {
                //HostShip.ShipInfo.UpgradeIcons.Upgrades.Add(upgradeType);
                HostShip.UpgradeBar.AddSlot(upgradeType);
            };

            HostShip.OnPreInstallUpgrade += OnPreInstallUpgrade;
            HostShip.OnRemovePreInstallUpgrade += OnRemovePreInstallUpgrade;
        }

        public override void ActivateAbility() { }

        public override void DeactivateAbility()
        {
            // Not required
        }

        public override void DeactivateAbilityForSquadBuilder() { }

        private void OnPreInstallUpgrade(GenericUpgrade upgrade)
        {
            if (HardpointSlotTypes.Contains(upgrade.UpgradeInfo.UpgradeTypes.First()))
            {
                HardpointSlotTypes
                    .Where(slot => slot != upgrade.UpgradeInfo.UpgradeTypes.First())
                    .ToList()
                    .ForEach(slot => HostShip.UpgradeBar.RemoveSlot(slot));
            }
        }

        private void OnRemovePreInstallUpgrade(GenericUpgrade upgrade)
        {
            if (HardpointSlotTypes.Contains(upgrade.UpgradeInfo.UpgradeTypes.First()))
            {
                HardpointSlotTypes
                    .Where(slot => slot != upgrade.UpgradeInfo.UpgradeTypes.First())
                    .ToList()
                    .ForEach(slot => HostShip.UpgradeBar.AddSlot(slot));
            }
        }
    }
}