using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.XWing
    {
        public class CavernAngelsZealot : XWing
        {
            public CavernAngelsZealot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Cavern Angels Zealot",
                    1,
                    41
                );

                ModelInfo.SkinName = "Partisan";

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Illicit);

                SEImageNumber = 12;
            }
        }
    }
}
