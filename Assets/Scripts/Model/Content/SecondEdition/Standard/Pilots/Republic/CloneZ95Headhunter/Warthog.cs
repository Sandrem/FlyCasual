using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.CloneZ95Headhunter
    {
        public class Warthog : CloneZ95Headhunter
        {
            public Warthog() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Warthog\"",
                    "Veteran of Kadavo",
                    Faction.Republic,
                    3,
                    3,
                    10,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.HawkAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Clone
                    }
                );

                PilotNameCanonical = "warthog-clonez95headhunter";

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/warthog-clonez95headhunter.png";
            }
        }
    }
}