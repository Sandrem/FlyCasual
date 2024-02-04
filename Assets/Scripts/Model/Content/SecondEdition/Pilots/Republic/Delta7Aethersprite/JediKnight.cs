using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.Delta7Aethersprite
{
    public class JediKnight : Delta7Aethersprite
    {
        public JediKnight()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Jedi Knight",
                "",
                Faction.Republic,
                3,
                4,
                3,
                force: 1,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.ForcePower,
                    UpgradeType.Astromech,
                    UpgradeType.Configuration,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Jedi,
                    Tags.LightSide
                }
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/b0/b3/b0b3f463-a3ea-4fe6-be69-41afed1b4110/swz32_jedi-knight.png";
        }
    }
}