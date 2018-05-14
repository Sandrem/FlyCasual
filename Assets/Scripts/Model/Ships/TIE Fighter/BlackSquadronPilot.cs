using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFighter
    {
        public class BlackSquadronPilot: TIEFighter, ISecondEditionPilot
        {
            public BlackSquadronPilot() : base()
            {
                PilotName = "Black Squadron Pilot";
                PilotSkill = 4;
                Cost = 14;
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotName = "Black Squadron Ace";
                PilotSkill = 3;
                ImageUrl = "https://i.imgur.com/uj0phgF.png";

                Cost = 30;
            }
        }
    }
}
