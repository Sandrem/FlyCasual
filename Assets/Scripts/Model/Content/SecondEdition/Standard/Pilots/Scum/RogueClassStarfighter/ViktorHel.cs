using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RogueClassStarfighter
    {
        public class ViktorHel : RogueClassStarfighter
        {
            public ViktorHel() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Viktor Hel",
                    "Storied Bounty Hunter",
                    Faction.Scum,
                    4,
                    5,
                    16,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ViktorHelAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Cannon,
                        UpgradeType.Cannon,
                        UpgradeType.Missile,
                        UpgradeType.Illicit,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>()
                    {
                        Tags.BountyHunter
                    }
                );

                PilotNameCanonical = "viktorhel-rogueclassstarfighter";

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/viktorhel-rogueclassstarfighter.png";
            }
        }
    }
}