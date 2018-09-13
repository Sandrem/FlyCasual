using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace AttackShuttle
    {
        public class HeraSyndulla : AttackShuttle, ISecondEditionPilot
        {
            public HeraSyndulla() : base()
            {
                PilotName = "Hera Syndulla";
                PilotSkill = 7;
                Cost = 22;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.HeraSyndullaAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                Cost = 39;
                PilotSkill = 5;

                SEImageNumber = 34;
            }
        }
    }
}
