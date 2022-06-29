using Content;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.GauntletFighter
    {
        public class BoKatanKryzeRepublic : GauntletFighter
        {
            public BoKatanKryzeRepublic() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "Bo-Katan Kryze",
                    "Nite Owl Commander",
                    Faction.Republic,
                    4,
                    7,
                    20,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.BoKatanKryzeRepublicAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Torpedo,
                        UpgradeType.Missile,
                        UpgradeType.Missile,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Modification,
                        UpgradeType.Modification,
                        UpgradeType.Configuration,
                        UpgradeType.Title
                    },
                    tags: new List<Tags>()
                    {
                        Tags.Mandalorian
                    }
                );

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/b/b7/Bokatankryze-republic.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BoKatanKryzeRepublicAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}