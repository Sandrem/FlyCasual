using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace FirstEdition.YT2400
    {
        public class WildSpaceFringer : YT2400
        {
            public WildSpaceFringer() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Wild Space Fringer",
                    2,
                    30
                );
            }
        }
    }
}
