using ActionsList;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ2AWing
    {
        public class TallissanLintra : RZ2AWing
        {
            public TallissanLintra() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Tallissan Lintra",
                    5,
                    35,
                    isLimited: true,
                    // abilityType: typeof(Abilities.SecondEdition.JakeFarrellAbility),
                    extraUpgradeIcons: new List<UpgradeType> { UpgradeType.Talent, UpgradeType.Talent } //,
                    //seImageNumber: 19
                );

                ModelInfo.SkinName = "Blue";

                ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/e/ef/Swz22_tallissan_lintra.png";
            }
        }
    }
}