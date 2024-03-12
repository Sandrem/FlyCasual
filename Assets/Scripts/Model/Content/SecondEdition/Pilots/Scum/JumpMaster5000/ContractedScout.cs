using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.JumpMaster5000
    {
        public class ContractedScout : JumpMaster5000
        {
            public ContractedScout() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Contracted Scout",
                    "",
                    Faction.Scum,
                    2,
                    5,
                    4,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Cannon,
                        UpgradeType.Torpedo,
                        UpgradeType.Illicit
                    },
                    seImageNumber: 217
                );
            }
        }
    }
}