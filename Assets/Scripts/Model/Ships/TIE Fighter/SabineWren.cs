using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;
using RuleSets;

namespace Ship
{
    namespace TIEFighter
    {
        public class SabineWren : TIEFighter, ISecondEditionPilot
        {
            public SabineWren() : base()
            {
                PilotName = "Sabine Wren";
                PilotSkill = 5;
                Cost = 15;

                faction = Faction.Rebel;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new SabineWrenPilotAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 3;
            }
        }
    }
}
