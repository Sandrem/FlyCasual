using Content;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.GauntletFighter
    {
        public class Maul : GauntletFighter
        {
            public Maul() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "Maul",
                    "Lord of the Shadow Collective",
                    Faction.Scum,
                    5,
                    9,
                    20,
                    force: 3,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.MaulAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.ForcePower,
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Missile,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Modification,
                        UpgradeType.Configuration,
                        UpgradeType.Title
                    },
                    tags: new List<Tags>()
                    {
                        Tags.DarkSide 
                    },
                    skinName: "Red Old"
                );

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/a/a2/Maulgauntlet.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MaulAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}