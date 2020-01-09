﻿using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESkStriker
    {
        public class BlackSquadronScout : TIESkStriker
        {
            public BlackSquadronScout() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Black Squadron Scout",
                    3,
                    34,
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 120
                );
            }
        }
    }
}
