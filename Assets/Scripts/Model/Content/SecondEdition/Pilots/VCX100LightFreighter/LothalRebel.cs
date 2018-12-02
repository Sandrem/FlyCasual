using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.VCX100LightFreighter
    {
        public class LothalRebel : VCX100LightFreighter
        {
            public LothalRebel() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Lothal Rebel",
                    2,
                    70,
                    seImageNumber: 76
                );
            }
        }
    }
}
