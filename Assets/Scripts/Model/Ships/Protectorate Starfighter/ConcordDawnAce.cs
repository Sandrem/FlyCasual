using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace ProtectorateStarfighter
    {
        public class ConcordDawnAce : ProtectorateStarfighter
        {
            public ConcordDawnAce() : base()
            {
                PilotName = "Concord Dawn Ace";
                PilotSkill = 5;
                Cost = 24;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}
