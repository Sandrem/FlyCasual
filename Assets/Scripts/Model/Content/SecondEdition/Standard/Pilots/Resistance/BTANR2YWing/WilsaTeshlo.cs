using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTANR2YWing
    {
        public class WilsaTeshlo : BTANR2YWing
        {
            public WilsaTeshlo() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "Wilsa Teshlo",
                    "Veiled Sorority Privateer",
                    Faction.Resistance,
                    4,
                    3,
                    14,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.WilsaTeshloAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.YWing
                    }
                );

                ImageUrl = "https://i.imgur.com/m8nrMvg.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class WilsaTeshloAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}
