using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace ProtectorateStarfighter
    {
        public class ConcordDawnVeteran : ProtectorateStarfighter
        {
            public ConcordDawnVeteran() : base()
            {
                PilotName = "Concord Dawn Veteran";
                PilotSkill = 3;
                Cost = 22;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}
