using Ship;
using Ship.JumpMaster5000;
using Upgrade;

namespace UpgradesList
{
    public class PunishingOne : GenericUpgrade
    {
        public PunishingOne() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Punishing One";
            Cost = 12;
            isUnique = true;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is JumpMaster5000;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);
            host.ChangeFirepowerBy(1);
        }
    }
}
