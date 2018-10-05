using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using RuleSets;

namespace Ship
{
    namespace EscapeCraft
    {
        public class L337EscapeCraft : EscapeCraft, ISecondEditionPilot
        {
            public L337EscapeCraft() : base()
            {
                PilotName = "L3-37";
                PilotSkill = 2;
                Cost = 22;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.SecondEdition.L337Ability());

                SEImageNumber = 228;
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not required
            }
        }
    }
}