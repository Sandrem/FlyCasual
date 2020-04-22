using Mods.ModsList;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ1AWing
    {
        public class SabineWren : RZ1AWing
        {
            public SabineWren() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Sabine Wren",
                    3,
                    40,
                    pilotTitle: "Spectre-5",
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.SabineWrenPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Talent, UpgradeType.Talent }
                );

                RequiredMods = new List<Type>() { typeof(PhoenixSquadronModSE) };
                PilotNameCanonical = "sabinewren-rz1awing-phoenixsquadronmod";

                ImageUrl = "https://i.imgur.com/YpP14NT.png";

                ModelInfo.SkinName = "Phoenix Squadron";

                ModelInfo.SkinName = "Green";
            }
        }
    }
}