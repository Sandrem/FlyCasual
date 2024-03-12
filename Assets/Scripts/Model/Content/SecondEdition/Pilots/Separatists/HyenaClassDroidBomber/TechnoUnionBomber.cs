﻿using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.HyenaClassDroidBomber
{
    public class TechnoUnionBomber : HyenaClassDroidBomber
    {
        public TechnoUnionBomber()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Techno Union Bomber",
                "",
                Faction.Separatists,
                1,
                3,
                6,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Torpedo,
                    UpgradeType.Device,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                    Tags.Droid
                }
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/7b/20/7b2032d2-dc96-4b69-9c59-92c809ba3da5/swz41_techno-union-bomber.png";
        }
    }
}