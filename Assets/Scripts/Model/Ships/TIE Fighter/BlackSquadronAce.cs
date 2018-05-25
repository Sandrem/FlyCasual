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
                ImageUrl = "https://i.imgur.com/gJajYw8.png";
                Cost = 30;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotRuleType = typeof(SecondEdition);
            }

            public void AdaptPilotToSecondEdition()
            {
                //No changes
            }
        }
    }
}
