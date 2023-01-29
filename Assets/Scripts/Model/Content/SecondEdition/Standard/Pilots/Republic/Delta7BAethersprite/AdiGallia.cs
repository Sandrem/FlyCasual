using System.Collections.Generic;
using Upgrade;
using Content;

namespace Ship.SecondEdition.Delta7BAethersprite
{
    public class AdiGallia : Delta7BAethersprite
    {
        public AdiGallia()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Adi Gallia",
                "Shooting Star",
                Faction.Republic,
                5,
                7,
                18,
                isLimited: true,
                force: 2,
                abilityType: typeof(Abilities.SecondEdition.AdiGalliaAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent
                },
                tags: new List<Tags>
                {
                    Tags.Jedi,
                    Tags.LightSide
                },
                skinName: "Plo Koon"
            );

            PilotNameCanonical = "adagallia-delta7baethersprite";

            ImageUrl = "https://infinitearenas.com/xw2/images/pilots/adigallia-delta7baethersprite.png";
        }
    }
}