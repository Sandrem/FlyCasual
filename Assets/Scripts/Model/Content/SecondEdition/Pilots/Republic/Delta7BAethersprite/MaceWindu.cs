using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.Delta7BAethersprite
{
    public class MaceWindu7B : Delta7BAethersprite
    {
        public MaceWindu7B()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Mace Windu",
                "Harsh Traditionalist",
                Faction.Republic,
                4,
                5,
                7,
                isLimited: true,
                force: 3,
                abilityType: typeof(Abilities.SecondEdition.MaceWinduAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.ForcePower,
                    UpgradeType.ForcePower,
                    UpgradeType.Astromech,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Jedi,
                    Tags.LightSide
                },
                skinName: "Mace Windu"
            );

            PilotNameCanonical = "macewindu-delta7baethersprite";

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/de/33/de3326f7-521c-4f50-8599-483db5f32d6d/swz32_mace-windu.png";
        }
    }
}