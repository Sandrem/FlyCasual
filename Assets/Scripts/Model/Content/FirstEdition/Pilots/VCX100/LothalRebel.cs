using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace FirstEdition.VCX100
    {
        public class LothalRebel : VCX100
        {
            public LothalRebel() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Lothal Rebel",
                    3,
                    35
                );
            }
        }
    }
}
