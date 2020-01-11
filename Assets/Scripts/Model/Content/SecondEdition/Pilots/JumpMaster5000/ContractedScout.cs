﻿using Upgrade;

namespace Ship
{
    namespace SecondEdition.JumpMaster5000
    {
        public class ContractedScout : JumpMaster5000
        {
            public ContractedScout() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Contracted Scout",
                    2,
                    41,
                    seImageNumber: 217
                );
            }
        }
    }
}