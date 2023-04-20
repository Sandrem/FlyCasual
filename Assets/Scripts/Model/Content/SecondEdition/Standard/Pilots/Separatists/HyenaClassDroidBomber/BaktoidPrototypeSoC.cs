using System;
using System.Collections.Generic;
using System.Linq;
using Content;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.HyenaClassDroidBomber
{
    public class BaktoidPrototypeSoC : HyenaClassDroidBomber
    {
        public BaktoidPrototypeSoC()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Baktoid Prototype",
                "Siege of Coruscant",
                Faction.Separatists,
                1,
                3,
                0,
                limited: 2,
                abilityType: typeof(Abilities.SecondEdition.BaktoidPrototypeAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Missile,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                    Tags.Droid
                },
                isStandardLayout: true
            );
            
            MustHaveUpgrades.Add(typeof(HomingMissiles));
            MustHaveUpgrades.Add(typeof(ContingencyProtocol));
            MustHaveUpgrades.Add(typeof(StrutLockOverride));

            ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/6/6c/Baktoidprototype-siegeofcoruscant.png";

            PilotNameCanonical = "baktoidprototype-siegeofcoruscant";
        }
    }
}