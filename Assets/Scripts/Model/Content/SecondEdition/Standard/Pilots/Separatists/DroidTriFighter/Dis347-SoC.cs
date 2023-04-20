using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.DroidTriFighter
{
    public class Dis347SoC : DroidTriFighter
    {
        public Dis347SoC()
        {
            PilotInfo = new PilotCardInfo25
            (
                "DIS-347",
                "Siege of Coruscant",
                Faction.Separatists,
                3,
                4,
                0,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.Dis347Ability),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Configuration,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Droid
                },
                isStandardLayout: true
            );

            MustHaveUpgrades.Add(typeof(Marksmanship));
            MustHaveUpgrades.Add(typeof(AfterBurners));
            MustHaveUpgrades.Add(typeof(ContingencyProtocol));

            ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/c/ce/Dis347-siegeofcoruscant.png";

            PilotNameCanonical = "dis347-siegeofcoruscant";
        }
    }
}