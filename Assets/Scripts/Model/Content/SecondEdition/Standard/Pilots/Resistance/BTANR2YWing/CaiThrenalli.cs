using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTANR2YWing
    {
        public class CaiThrenalli : BTANR2YWing
        {
            public CaiThrenalli() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "C'ai Threnalli",
                    "Tenacious Survivor",
                    Faction.Resistance,
                    2,
                    3,
                    15,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CaiThrenalliAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.YWing
                    }
                );

                PilotNameCanonical = "caithrenalli-btanr2ywing";

                ImageUrl = "https://i.imgur.com/quuZGXf.png";
            }
        }
    }
}
