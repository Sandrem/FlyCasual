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

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/72cb6c4e50b0ad24af0bb84ce0aa53f0.png";
            }
        }
    }
}