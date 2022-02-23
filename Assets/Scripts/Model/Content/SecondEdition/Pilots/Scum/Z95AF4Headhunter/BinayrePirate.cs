using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Z95AF4Headhunter
    {
        public class BinayrePirate : Z95AF4Headhunter
        {
            public BinayrePirate() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Binayre Pirate",
                    "",
                    Faction.Scum,
                    1,
                    3,
                    2,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Illicit
                    },
                    seImageNumber: 173,
                    skinName: "Binayre Pirate"
                );
            }
        }
    }
}
