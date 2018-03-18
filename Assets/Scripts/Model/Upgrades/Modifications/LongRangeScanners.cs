using Ship;
using Upgrade;

namespace UpgradesList
{
    public class LongRangeScanners : GenericUpgrade
    {
        public LongRangeScanners() : base()
        {
            Types.Add(UpgradeType.Modification);
            Name = "Long-Range Scanners";
            Cost = 0;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.UpgradeBar.HasUpgradeSlot(UpgradeType.Torpedo) && ship.UpgradeBar.HasUpgradeSlot(UpgradeType.Missile);
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);
            host.SetTargetLockRange(3, int.MaxValue);
        }
    }
}
