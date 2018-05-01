using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEReaper
    {
        public class CaptainFeroph : TIEReaper
        {
            public CaptainFeroph() : base()
            {
                PilotName = "Captain Feroph";
                PilotSkill = 4;
                Cost = 24;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}
