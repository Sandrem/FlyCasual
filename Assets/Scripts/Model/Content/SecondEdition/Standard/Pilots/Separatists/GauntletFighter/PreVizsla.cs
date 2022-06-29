using Content;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.GauntletFighter
    {
        public class PreVizsla : GauntletFighter
        {
            public PreVizsla() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "Pre Vizsla",
                    "Leader of Death Watch",
                    Faction.Separatists,
                    3,
                    7,
                    18,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.PreVizslaAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Missile,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Modification,
                        UpgradeType.Configuration
                    },
                    tags: new List<Tags>()
                    {
                        Tags.Mandalorian 
                    }
                );

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/1/1f/Previzsla.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PreVizslaAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}