using Ship;
using System;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.TIEInterceptor
    {
        public class SaberSquadronAce : TIEInterceptor
        {
            public SaberSquadronAce() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Saber Squadron Ace",
                    4,
                    40
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                ModelInfo.SkinName = "Red Stripes";

                SEImageNumber = 105;
            }
        }
    }
}