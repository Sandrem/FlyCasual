using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.Z95Headhunter
    {
        public class BinayrePirate : Z95Headhunter
        {
            public BinayrePirate() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Binayre Pirate",
                    1,
                    12,
                    extraUpgradeIcon: UpgradeType.Illicit,
                    factionOverride: Faction.Scum
                );

                ModelInfo.SkinName = "Binayre Pirate";
            }
        }
    }
}
