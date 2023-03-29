using Upgrade;
using UnityEngine;
using Ship;
using System.Collections.Generic;

namespace UpgradesList.SecondEdition
{
    public class R2D2BoY : GenericUpgrade
    {
        public R2D2BoY() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo(
                "R2-D2",
                UpgradeType.Astromech,
                cost: 0,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.R2AstromechAbility),
                charges: 2
            );

            ImageUrl = "https://i.imgur.com/pgrul0D.jpg";

            NameCanonical = "r2d2-battleofyavin";
        }
    }
}