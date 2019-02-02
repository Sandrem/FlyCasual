using Ship;
using Upgrade;
using System.Collections.Generic;

namespace UpgradesList.FirstEdition
{
    public class PunishingOne : GenericUpgrade
    {
        public PunishingOne() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Punishing One",
                UpgradeType.Title,
                cost: 12,
                isLimited: true,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.JumpMaster5000.JumpMaster5000)),
                abilityType: typeof(Abilities.FirstEdition.PunishingOneAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
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