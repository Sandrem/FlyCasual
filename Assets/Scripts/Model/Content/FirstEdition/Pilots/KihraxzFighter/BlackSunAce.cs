using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.KihraxzFighter
    {
        public class BlackSunAce : KihraxzFighter
        {
            public BlackSunAce() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Black Sun Ace",
                    5,
                    23
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                ModelInfo.SkinName = "Black Sun (White)";
            }
        }
    }
}
