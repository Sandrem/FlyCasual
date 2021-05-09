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
                    abilityType: typeof(Abilities.SecondEdition.HeraSyndullaABWingAbility),
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Talent, UpgradeType.Talent }
                );

                PilotNameCanonical = "herasyndulla-rz1awing";

                ModelInfo.SkinName = "Hera Syndulla";

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/d/dc/Herasyndullaawing.png";
            }
        }
    }
}