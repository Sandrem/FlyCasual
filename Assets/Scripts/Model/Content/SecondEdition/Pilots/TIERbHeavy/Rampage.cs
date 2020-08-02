using System.Collections;
using System.Collections.Generic;
using BoardTools;
using Arcs;
using Upgrade;
using Ship;
using System;

namespace Ship
{
    namespace SecondEdition.TIERbHeavy
    {
        public class Rampage : TIERbHeavy
        {
            public Rampage() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Rampage\"",
                    4,
                    60,
                    isLimited: true
                );

                ImageUrl = "https://i.imgur.com/cPVLRxm.png";
            }
        }
    }
}