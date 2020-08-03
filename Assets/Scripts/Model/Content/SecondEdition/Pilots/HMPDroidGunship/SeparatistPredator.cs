using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.HMPDroidGunship
    {
        public class SeparatistPredator : HMPDroidGunship
        {
            public SeparatistPredator() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Separatist Predator",
                    4,
                    50
                );
            }
        }
    }
}