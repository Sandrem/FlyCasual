using Content;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.GauntletFighter
    {
        public class CaptainHark : GauntletFighter
        {
            public CaptainHark() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "Captain Hark",
                    "Obedient Underling",
                    Faction.Imperial,
                    3,
                    7,
                    15,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CaptainHarkAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
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

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/1/10/Captainhark.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CaptainHarkAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}