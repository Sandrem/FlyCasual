using Ship;
using Ship.M12LKimogila;
using Upgrade;

namespace UpgradesList
{
    public class Enforcer : GenericUpgrade
    {
        public Enforcer() : base()
        {
            Type = UpgradeType.Title;
            Name = "Enforcer";
            Cost = 1;

            isUnique = true;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is M12LKimogila;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);
        }
    }
}
