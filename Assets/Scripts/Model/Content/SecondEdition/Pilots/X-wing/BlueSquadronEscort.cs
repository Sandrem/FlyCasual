using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace SecondEdition.XWing
    {
        public class BlueSquadronEscort : XWing
        {
            public BlueSquadronEscort() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Blue Squadron Escort",
                    2,
                    41
                );

                SEImageNumber = 11;
            }
        }
    }
}
