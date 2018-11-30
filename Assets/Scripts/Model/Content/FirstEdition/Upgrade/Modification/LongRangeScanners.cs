using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class LongRangeScanners : GenericUpgrade
    {
        public LongRangeScanners() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Long Range Scanners",
                UpgradeType.Modification,
                cost: 0,
                abilityType: typeof(Abilities.FirstEdition.LongRangeScannersAbility),
                restriction: new UpgradeBarRestriction(UpgradeType.Torpedo, UpgradeType.Missile)
            );
        }
    }
}

namespace Abilities.FirstEdition
{
    public class LongRangeScannersAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.SetTargetLockRange(3, int.MaxValue);
        }

        public override void DeactivateAbility()
        {
            HostShip.SetTargetLockRange(1, 3);
        }
    }
}