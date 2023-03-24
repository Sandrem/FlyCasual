using Abilities.SecondEdition;
using Content;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship
{
    namespace SecondEdition.TIEAdvancedX1
    {
        public class DarthVaderBoY : TIEAdvancedX1
        {
            public DarthVaderBoY() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Darth Vader",
                    "Battle of Yavin",
                    Faction.Imperial,
                    6,
                    6,
                    0,
                    isLimited: true,
                    abilityType: typeof(DarthVaderDefenderAbility),
                    force: 3,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.ForcePower,
                        UpgradeType.Talent,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie,
                        Tags.DarkSide,
                        Tags.Sith
                    },
                    skinName: "Blue",
                    isStandardLayout: true
                );

                DefaultUpgrades.Add(typeof(Marksmanship));
                DefaultUpgrades.Add(typeof(Hate));
                DefaultUpgrades.Add(typeof(AfterBurners));

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/a/a9/Darthvader-battleofyavin.png";

                PilotNameCanonical = "darthvader-battleofyavin";
            }
        }
    }
}