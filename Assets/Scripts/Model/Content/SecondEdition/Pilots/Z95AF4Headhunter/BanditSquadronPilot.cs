﻿using Editions;

namespace Ship
{
    namespace SecondEdition.Z95AF4Headhunter
    {
        public class BanditSquadronPilot : Z95AF4Headhunter
        {
            public BanditSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Bandit Squadron Pilot",
                    1,
                    22,
                    seImageNumber: 30
                );
            }
        }
    }
}
