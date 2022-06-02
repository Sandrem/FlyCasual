using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.Delta7BAethersprite
{
    public class PloKoon7B : Delta7BAethersprite
    {
        public PloKoon7B()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Plo Koon",
                "Serene Mentor",
                Faction.Republic,
                5,
                7,
                17,
                isLimited: true,
                force: 2,
                abilityType: typeof(Abilities.SecondEdition.PloKoonAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.ForcePower
                },
                tags: new List<Tags>
                {
                    Tags.Jedi,
                    Tags.LightSide
                },
                skinName: "Plo Koon"
            );

            PilotNameCanonical = "plokoon-delta7baethersprite";

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/6a/6f/6a6fef51-fb5f-49c1-b5cc-8e96b6d09051/swz32_plo-koon.png";
        }
    }
}