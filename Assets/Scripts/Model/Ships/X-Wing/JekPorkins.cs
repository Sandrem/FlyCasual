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
        public class JekPorkins : XWing, ISecondEditionPilot
        {
            public JekPorkins() : base()
            {
                PilotName = "Jek Porkins";
                PilotSkill = 4;
                Cost = 60;

                IsUnique = true;

                ShipRuleType = typeof(SecondEdition);

                ImageUrl = "https://i.imgur.com/xoIkP6d.png";

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }

            public void AdaptPilotToSecondEdition()
            {
                //No adaptation is reqiored
            }
        }
    }
}