using Upgrade;
using UnityEngine;
using Ship;
using System.Collections.Generic;
using System;
using Tokens;

namespace UpgradesList.SecondEdition
{
    public class R2F2BoY : GenericUpgrade
    {
        public R2F2BoY() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo(
                "R2-F2",
                UpgradeType.Astromech,
                cost: 0,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.R2AstromechBoYAbility),
                charges: 2
            );

            ImageUrl = "https://i.imgur.com/g9vlF6c.jpg";

            NameCanonical = "r2f2-battleofyavin";
        }
    }
}