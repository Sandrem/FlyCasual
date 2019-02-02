﻿using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEFoFighter
    {
        public class OmegaSquadronPilot : TIEFoFighter
        {
            public OmegaSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Omega Squadron Pilot",
                    4,
                    17,
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}
