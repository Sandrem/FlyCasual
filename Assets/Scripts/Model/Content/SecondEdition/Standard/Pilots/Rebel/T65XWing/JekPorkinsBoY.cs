using Abilities.SecondEdition;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class JekPorkinsBoY : T65XWing
        {
            public JekPorkinsBoY() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Jek Porkins",
                    "Battle of Yavin",
                    Faction.Rebel,
                    4,
                    4,
                    0,
                    isLimited: true,
                    abilityType: typeof(JekPorkinsAbility),
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
                    skinName: "Jek Porkins",
                    isStandardLayout: true
                );

                ShipAbilities.Add(new HopeAbility());

                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.AdvProtonTorpedoes));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.R5D8));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.UnstableSublightEngines));

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/1/1b/Jekporkins-battleofyavin.png";

                PilotNameCanonical = "jekporkins-battleofyavin";
            }
        }
    }
}