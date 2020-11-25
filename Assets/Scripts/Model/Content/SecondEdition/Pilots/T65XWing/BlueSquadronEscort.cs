using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class BlueSquadronEscort : T65XWing
        {
            public BlueSquadronEscort() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Blue Squadron Escort",
                    2,
                    38,
                    seImageNumber: 11
                );

                ModelInfo.SkinName = "Blue";
            }
        }
    }
}
