﻿using Editions;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class BlackSquadronAce : TIELnFighter
        {
            public BlackSquadronAce() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Black Squadron Ace",
                    3,
                    26,
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 90
                );
            }
        }
    }
}
