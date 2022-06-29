using Content;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.GauntletFighter
    {
        public class Chopper : GauntletFighter
        {
            public Chopper() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Chopper\"",
                    "Spectre-3",
                    Faction.Rebel,
                    2,
                    7,
                    18,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ChopperPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Modification,
                        UpgradeType.Modification,
                        UpgradeType.Configuration,
                        UpgradeType.Title
                    },
                    tags: new List<Tags>()
                    {
                        Tags.Droid,
                        Tags.Spectre
                    },
                    skinName: "Red"
                );

                ShipInfo.ActionIcons.SwitchToDroidActions();

                PilotNameCanonical = "chopper-gauntletfighter";

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/4/45/Choppergauntlet.png";
            }
        }
    }
}