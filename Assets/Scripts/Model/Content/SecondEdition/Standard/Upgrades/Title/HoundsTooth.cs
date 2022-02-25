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
                cost: 0,
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
            ActivateDocking(FilterZ95);
        }

        public override void DeactivateAbility()
        {
            DeactivateDocking();
        }

        private bool FilterZ95(GenericShip ship)
        {
            return ship is Ship.SecondEdition.Z95AF4Headhunter.Z95AF4Headhunter;
        }
    }
}