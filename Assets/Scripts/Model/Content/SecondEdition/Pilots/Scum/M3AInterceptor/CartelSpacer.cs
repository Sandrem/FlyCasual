using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.M3AInterceptor
    {
        public class CartelSpacer : M3AInterceptor
        {
            public CartelSpacer() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Cartel Spacer",
                    "",
                    Faction.Scum,
                    1,
                    3,
                    4,
                    extraUpgradeIcons: new List<UpgradeType>
                    { 
                        UpgradeType.Modification
                    },
                    seImageNumber: 190
                );
            }
        }
    }
}