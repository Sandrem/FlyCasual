using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.CloneZ95Headhunter
    {
        public class Hawk : CloneZ95Headhunter
        {
            public Hawk() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Hawk\"",
                    "Valkyrie 2929",
                    Faction.Republic,
                    4,
                    3,
                    8,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.HawkAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Modification,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Clone
                    }
                );

                PilotNameCanonical = "hawk-clonez95headhunter";

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/hawk-clonez95headhunter.png";
            }
        }
    }
}