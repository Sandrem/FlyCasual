using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace XWing
    {
        public class CavernAngelsZealot : XWing
        {
            public CavernAngelsZealot() : base()
            {
                PilotName = "Cavern Angels Zealot";
                PilotSkill = 1;
                Cost = 22;

                SkinName = "Partisan";

                PrintedUpgradeIcons.Add(UpgradeType.Elite);
            }
        }
    }
}
