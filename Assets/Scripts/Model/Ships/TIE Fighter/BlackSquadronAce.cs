using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFighter
    {
        public class BlackSquadronAce : TIEFighter, ISecondEditionPilot
        {
            public BlackSquadronAce() : base()
            {
                PilotName = "Black Squadron Ace";
                PilotSkill = 3;
                Cost = 26;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotRuleType = typeof(SecondEdition);

                SEImageNumber = 90;
            }

            public void AdaptPilotToSecondEdition()
            {
                //No changes
            }
        }
    }
}
