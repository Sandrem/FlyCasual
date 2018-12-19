using Editions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.XWing
    {
        public class CavernAngelsZealot : XWing
        {
            public CavernAngelsZealot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Cavern Angels Zealot",
                    1,
                    22,
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ModelInfo.SkinName = "Partisan";
            }
        }
    }
}
