using System.Collections.Generic;
using Upgrade;
using Content;

namespace Ship.SecondEdition.Delta7BAethersprite
{
    public class AnakinSkywalker7B : Delta7BAethersprite
    {
        public AnakinSkywalker7B()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Anakin Skywalker",
                "Hero of the Republic",
                Faction.Republic,
                6,
                7,
                15,
                true,
                force: 3,
                abilityType: typeof(Abilities.SecondEdition.AnakinSkywalkerAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.ForcePower,
                    UpgradeType.Talent
                },
                tags: new List<Tags>
                {
                    Tags.Jedi,
                    Tags.LightSide
                },
                skinName: "Anakin Skywalker"
            );

            PilotNameCanonical = "anakinskywalker-delta7baethersprite";

            ImageUrl = "https://squadbuilder.fantasyflightgames.com/card_images/en/d60f4eca355471465ca3f6b99fb98e56.png";
        }
    }
}