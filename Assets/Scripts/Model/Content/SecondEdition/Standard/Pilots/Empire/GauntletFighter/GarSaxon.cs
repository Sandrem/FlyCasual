using Content;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.GauntletFighter
    {
        public class GarSaxon : GauntletFighter
        {
            public GarSaxon() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "Gar Saxon",
                    "Treacherous Viceroy",
                    Faction.Imperial,
                    3,
                    8,
                    18,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.GarSaxonAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Torpedo,
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

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/7/79/Garsaxon.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GarSaxonAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}