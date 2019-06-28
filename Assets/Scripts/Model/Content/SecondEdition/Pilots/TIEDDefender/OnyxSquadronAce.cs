﻿using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEDDefender
    {
        public class OnyxSquadronAce : TIEDDefender
        {
            public OnyxSquadronAce() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Onyx Squadron Ace",
                    4,
                    75,
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 125
                );
            }
        }
    }
}
