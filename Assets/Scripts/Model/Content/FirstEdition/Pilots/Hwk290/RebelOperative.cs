using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace FirstEdition.Hwk290
    {
        public class RebelOperative : Hwk290
        {
            public RebelOperative() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Rebel Operative",
                    2,
                    16
                );
            }
        }
    }
}
