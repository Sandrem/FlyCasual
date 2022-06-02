using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.Delta7BAethersprite
{
    public class ObiWanKenobi7B : Delta7BAethersprite
    {
        public ObiWanKenobi7B()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Obi-Wan Kenobi",
                "Guardian of the Republic",
                Faction.Republic,
                5,
                7,
                15,
                true,
                force: 3,
                abilityType: typeof(Abilities.SecondEdition.ObiWanKenobiAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.ForcePower,
                    UpgradeType.Talent
                },
                tags: new List<Tags>
                {
                    Tags.Jedi,
                    Tags.LightSide
                }
            );

            PilotNameCanonical = "obiwankenobi-delta7baethersprite";

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/f9/24/f9246e39-4852-4a8f-a331-9b78f62439e9/swz32_obi-wan-kenobi.png";
        }
    }
}