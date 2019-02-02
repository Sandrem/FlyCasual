using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class ST321 : GenericUpgrade
    {
        public ST321() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "ST-321",
                UpgradeType.Title,
                cost: 3,
                isLimited: true,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.LambdaClassShuttle.LambdaClassShuttle)),
                abilityType: typeof(Abilities.FirstEdition.ST321Ability)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class ST321Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.SetTargetLockRange(1, int.MaxValue);
        }

        public override void DeactivateAbility()
        {
            HostShip.SetTargetLockRange(1, 3);
        }
    }
}