﻿using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.Delta7BAethersprite
{
    public class LuminaraUnduli7B : Delta7BAethersprite
    {
        public LuminaraUnduli7B()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Luminara Unduli",
                "Wise Protector",
                Faction.Republic,
                4,
                6,
                7,
                isLimited: true,
                force: 2,
                abilityType: typeof(Abilities.SecondEdition.LuminaraUnduliAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.ForcePower,
                    UpgradeType.Astromech,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Jedi,
                    Tags.LightSide
                },
                skinName: "Green"
            );

            PilotNameCanonical = "luminaraunduli-delta7baethersprite";
        }
    }
}