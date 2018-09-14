using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace SheathipedeShuttle
    {
        public class ZebOrrelios : SheathipedeShuttle, ISecondEditionPilot
        {
            public ZebOrrelios() : base()
            {
                PilotName = "\"Zeb\" Orrelios";
                PilotSkill = 3;
                Cost = 16;

                IsUnique = true;

                PilotAbilities.Add(new Abilities.ZebOrreliosPilotAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                Cost = 32;
                PilotSkill = 2;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SEImageNumber = 40;
            }
        }
    }
}
