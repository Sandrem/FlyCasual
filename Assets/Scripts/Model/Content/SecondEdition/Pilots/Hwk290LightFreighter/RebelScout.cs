using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace SecondEdition.Hwk290LightFreighter
    {
        public class RebelScout : Hwk290LightFreighter
        {
            public RebelScout() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Rebel Scout",
                    2,
                    32
                );

                SEImageNumber = 45;
            }
        }
    }
}
