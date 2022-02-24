using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.UT60DUWing
    {
        public class BlueSquadronScout : UT60DUWing
        {
            public BlueSquadronScout() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Blue Squadron Scout",
                    "",
                    Faction.Rebel,
                    2,
                    5,
                    6,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Modification
                    },
                    seImageNumber: 60
                );
            }
        }
    }
}
