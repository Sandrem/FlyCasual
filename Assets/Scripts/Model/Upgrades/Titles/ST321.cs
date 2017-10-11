using Ship;
using Ship.LambdaShuttle;
using Upgrade;

namespace UpgradesList
{
    public class ST321 : GenericUpgrade
    {
        public ST321() : base()
        {
            Type = UpgradeType.Title;
            Name = "ST-321";
            Cost = 3;
            isUnique = true;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is LambdaShuttle;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);
            host.SetTargetLockRange(1, int.MaxValue);
        }
    }
}
