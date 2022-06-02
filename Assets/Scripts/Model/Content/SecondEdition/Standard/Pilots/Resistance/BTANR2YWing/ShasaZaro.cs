using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTANR2YWing
    {
        public class ShasaZaro : BTANR2YWing
        {
            public ShasaZaro() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "Shasa Zaro",
                    "Artistic Ace",
                    Faction.Resistance,
                    3,
                    3,
                    13,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ShasaZaroAbility),
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

                ImageUrl = "https://i.imgur.com/AL8m0H5.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ShasaZaroAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}
