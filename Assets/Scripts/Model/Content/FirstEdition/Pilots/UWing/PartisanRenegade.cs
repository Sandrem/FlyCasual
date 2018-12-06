using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace FirstEdition.UWing
    {
        public class PartisanRenegade : UWing
        {
            public PartisanRenegade() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Partisan Renegade",
                    1,
                    22
                );

                ModelInfo.SkinName = "Partisan";
            }
        }
    }
}
