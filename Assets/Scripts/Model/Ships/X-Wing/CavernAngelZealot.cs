using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace XWing
    {
        public class CavernAngelZealot : XWing
        {
            public CavernAngelZealot() : base()
            {
                PilotName = "Cavern Angel Zealot";
                PilotSkill = 1;
                Cost = 22;

                SkinName = "Partisan";

                PrintedUpgradeIcons.Add(UpgradeType.Elite);
            }
        }
    }
}
