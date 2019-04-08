﻿using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class R5D8 : GenericUpgrade
    {
        public R5D8() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R5-D8",
                UpgradeType.Astromech,
                cost: 6,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.R5AstromechAbility),
                restriction: new FactionRestriction(Faction.Rebel),
                charges: 3,
                seImageNumber: 101
            );
        }
    }
}