using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System.Linq;
using Abilities;
using RuleSets;

namespace Ship
{
    namespace TIEFighter
    {
        public class GideonHask : TIEFighter, ISecondEditionPilot
        {
            public GideonHask() : base()
            {
                PilotName = "Gideon Hask";
                PilotSkill = 4;
                Cost = 30;

                IsUnique = true;
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotRuleType = typeof(SecondEdition);

                PilotAbilities.Add(new ScourgeAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not required
            }
        }
    }
}