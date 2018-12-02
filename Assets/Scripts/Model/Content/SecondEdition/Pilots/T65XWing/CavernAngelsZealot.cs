using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class CavernAngelsZealot : T65XWing
        {
            public CavernAngelsZealot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Cavern Angels Zealot",
                    1,
                    41,
                    extraUpgradeIcon: UpgradeType.Illicit,
                    seImageNumber: 12
                );

                ModelInfo.SkinName = "Partisan";
            }
        }
    }
}
