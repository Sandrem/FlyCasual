using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEAggressor
    {
        public class OnyxSquadronScout : TIEAggressor, ISecondEditionPilot
        {
            public OnyxSquadronScout() : base()
            {
                PilotName = "Onyx Squadron Scout";
                PilotSkill = 3;
                Cost = 19;
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 3;
                Cost = 32;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}
