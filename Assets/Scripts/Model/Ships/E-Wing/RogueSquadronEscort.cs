using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace EWing
    {
        public class RogueSquadronEscort : EWing, ISecondEditionPilot
        {
            public RogueSquadronEscort() : base()
            {
                PilotName = "Rogue Squadron Escort";
                PilotSkill = 4;
                Cost = 63;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotRuleType = typeof(SecondEdition);

                SEImageNumber = 52;
            }

            public void AdaptPilotToSecondEdition()
            {
                //Not required
            }
        }
    }
}
