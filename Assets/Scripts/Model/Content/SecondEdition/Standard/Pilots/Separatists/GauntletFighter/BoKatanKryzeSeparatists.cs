using Content;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.GauntletFighter
    {
        public class BoKatanKryzeSeparatists : GauntletFighter
        {
            public BoKatanKryzeSeparatists() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "Bo-Katan Kryze",
                    "Vizsla's Lieutenant",
                    Faction.Separatists,
                    4,
                    7,
                    20,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.BoKatanKryzeSeparatistsAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
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
                    },
                    skinName: "CIS"
                );

                PilotNameCanonical = "bokatankryze-separatistalliance";

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/a/a4/Bokatankryze-separatist-alliance.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BoKatanKryzeSeparatistsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}