using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ1AWing
    {
        public class HeraSyndulla : RZ1AWing
        {
            public HeraSyndulla() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Hera Syndulla",
                    "Phoenix Leader",
                    Faction.Rebel,
                    6,
                    5,
                    14,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.HeraSyndullaABWingAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.AWing,
                        Tags.Spectre
                    },
                    skinName: "Hera Syndulla"
                );

                PilotNameCanonical = "herasyndulla-rz1awing";

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/d/dc/Herasyndullaawing.png";
            }
        }
    }
}