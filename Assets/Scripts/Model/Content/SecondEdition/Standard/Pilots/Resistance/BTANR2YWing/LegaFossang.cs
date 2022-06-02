using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTANR2YWing
    {
        public class LegaFossang : BTANR2YWing
        {
            public LegaFossang() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "Lega Fossang",
                    "Hero of Humbarine",
                    Faction.Resistance,
                    3,
                    4,
                    19,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LegaFossangAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile
                    },
                    tags: new List<Tags>
                    {
                        Tags.YWing
                    }
                );

                ImageUrl = "https://i.imgur.com/SIFbsBi.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LegaFossangAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}
