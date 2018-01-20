using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace ScurrgH6Bomber
    {
        public class LokRevenant : ScurrgH6Bomber
        {
            public LokRevenant() : base()
            {
                PilotName = "Lok Revenant";
                PilotSkill = 3;
                Cost = 26;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}
