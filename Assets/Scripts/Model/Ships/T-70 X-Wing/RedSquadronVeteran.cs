using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace T70XWing
    {
        public class RedSquadronVeteran : T70XWing
        {
            public RedSquadronVeteran() : base()
            {
                PilotName = "Red Squadron Veteran";
                PilotSkill = 4;
                Cost = 26;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Red";
            }
        }
    }
}
