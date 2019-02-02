using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.Firespray31
    {
        public class MandalorianMercenary : Firespray31
        {
            public MandalorianMercenary() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Mandalorian Mercenary",
                    5,
                    35,
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Talent, UpgradeType.Illicit },
                    factionOverride: Faction.Scum
                );

                ModelInfo.SkinName = "Mandalorian Mercenary";
            }
        }
    }
}
