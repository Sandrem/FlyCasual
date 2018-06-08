using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;
using RuleSets;

namespace Ship
{
    namespace XWing
    {
        public class EdrioTwoTubes : XWing, ISecondEditionPilot
        {
            public EdrioTwoTubes() : base()
            {
                PilotName = "Edrio Two Tubes";
                PilotSkill = 2;
                Cost = 59;

                IsUnique = true;

                PilotRuleType = typeof(SecondEdition);

                //ImageUrl = "";

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }

            public void AdaptPilotToSecondEdition()
            {
                //No adaptation is reqiored
            }
        }
    }
}