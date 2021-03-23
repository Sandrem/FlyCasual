using Mods.ModsList;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ1AWing
    {
        public class HeraSyndulla : RZ1AWing
        {
            public HeraSyndulla() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Hera Syndulla",
                    6,
                    42,
                    pilotTitle: "Phoenix Leader",
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.HeraSyndullaBWingAbility),
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Talent, UpgradeType.Talent }
                );

                PilotNameCanonical = "herasyndulla-rz1awing";

                ModelInfo.SkinName = "Hera Syndulla";

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/da/1c/da1cbafb-3893-4ebb-aa80-516f21c2186a/swz83_ship_herasyndulla.png";
            }
        }
    }
}