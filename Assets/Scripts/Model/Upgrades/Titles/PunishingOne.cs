using Ship;
using Ship.JumpMaster5000;
using Upgrade;
using Abilities;

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

            UpgradeAbilities.Add(new PunishingOneAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is JumpMaster5000;
        }
    }
}

namespace Abilities
{
    public class PunishingOneAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.ChangeFirepowerBy(1);
        }

        public override void DeactivateAbility()
        {
            HostShip.ChangeFirepowerBy(-1);
        }
    }
}
