using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEBomber
    {
        public class GammaSquadronAce : TIEBomber, ISecondEditionPilot
        {
            public GammaSquadronAce() : base()
            {
                PilotName = "Gamma Squadron Ace";
                PilotSkill = 3;
                Cost = 30;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "White Stripes";

                PilotRuleType = typeof(SecondEdition);

                SEImageNumber = 111;
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not required
            }
        }
    }
}
