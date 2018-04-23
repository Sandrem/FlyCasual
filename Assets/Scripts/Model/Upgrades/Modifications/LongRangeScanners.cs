using Ship;
using Upgrade;
using Abilities;

namespace UpgradesList
{
    public class LongRangeScanners : GenericUpgrade
    {
        public LongRangeScanners() : base()
        {
            Types.Add(UpgradeType.Modification);
            Name = "Long-Range Scanners";
            Cost = 0;

            UpgradeAbilities.Add(new LongRangeScannersAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.UpgradeBar.HasUpgradeSlot(UpgradeType.Torpedo) && ship.UpgradeBar.HasUpgradeSlot(UpgradeType.Missile);
        }
    }
}

namespace Abilities
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
