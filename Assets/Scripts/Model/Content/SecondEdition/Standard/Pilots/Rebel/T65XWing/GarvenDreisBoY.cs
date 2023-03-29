using System.Collections.Generic;
using Upgrade;
using Abilities.SecondEdition;
using Content;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class GarvenDreisBoY : T65XWing
        {
            public GarvenDreisBoY() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Garven Dreis",
                    "Red Leader",
                    Faction.Rebel,
                    4,
                    4,
                    0,
                    isLimited: true,
                    abilityType: typeof(GarvenDreisXWingAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Torpedo,
                        UpgradeType.Astromech,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.XWing
                    },
                    isStandardLayout: true
                );

                ShipAbilities.Add(new HopeAbility());

                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.AdvProtonTorpedoes));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.R5K6BoY));

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/5/52/Garvendreis-battleofyavin.png";

                PilotNameCanonical = "garvendreis-battleofyavin";
            }
        }
    }
}