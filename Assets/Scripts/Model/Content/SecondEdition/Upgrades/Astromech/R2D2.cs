using Upgrade;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class R2D2 : GenericUpgrade
    {
        public R2D2() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R2-D2",
                UpgradeType.Astromech,
                cost: 8,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.R2AstromechAbility),
                restriction: new FactionRestriction(Faction.Rebel),
                charges: 3,
                seImageNumber: 100
            );
        }
    }
}