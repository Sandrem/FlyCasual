using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class HoundsTooth : GenericUpgrade
    {
        public HoundsTooth() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Hound's Tooth",
                UpgradeType.Title,
                cost: 1,
                isLimited: true,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.YV666LightFreighter.YV666LightFreighter)),
                abilityType: typeof(Abilities.SecondEdition.HoundsToothAbility),
                seImageNumber: 148
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class HoundsToothAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCanReleaseDockedShipRegular += DenyRelease;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCanReleaseDockedShipRegular -= DenyRelease;
        }

        private void DenyRelease(ref bool canRelease)
        {
            canRelease = false;
        }
    }
}