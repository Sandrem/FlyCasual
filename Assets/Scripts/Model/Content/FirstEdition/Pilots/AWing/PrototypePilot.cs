using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.AWing
    {
        public class PrototypePilot : AWing
        {
            public PrototypePilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Prototype Pilot",
                    1,
                    17
                );

                ModelInfo.SkinName = "Blue";
            }
        }
    }
}