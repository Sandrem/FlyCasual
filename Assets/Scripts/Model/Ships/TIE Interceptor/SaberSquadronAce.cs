using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEInterceptor
    {
        public class SaberSquadronAce : TIEInterceptor, ISecondEditionPilot
        {
            public SaberSquadronAce() : base()
            {
                PilotName = "Saber Squadron Ace";
                PilotSkill = 4;
                Cost = 21;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Red Stripes";

                PilotRuleType = typeof(SecondEdition);
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 4;
                Cost = 40;

                SEImageNumber = 105;
            }
        }
    }
}
