﻿using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.Delta7BAethersprite
{
    public class BarrissOffee7B : Delta7BAethersprite
    {
        public BarrissOffee7B()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Barriss Offee",
                "Conflicted Padawan",
                Faction.Republic,
                4,
                5,
                10,
                true,
                force: 1,
                abilityType: typeof(Abilities.SecondEdition.BarrissOffeeAbility),
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
                skinName: "Blue"
            );

            PilotNameCanonical = "barrissoffee-delta7baethersprite";
        }
    }
}