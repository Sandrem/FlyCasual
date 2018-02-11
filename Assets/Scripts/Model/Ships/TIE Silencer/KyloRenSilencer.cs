using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;
using Ship;

namespace Ship
{
    namespace TIESilencer
    {
        public class KyloRenSilencer : TIESilencer
        {
            public KyloRenSilencer() : base()
            {
                PilotName = "Kylo Ren";
                PilotSkill = 9;
                Cost = 35;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new KyloRenPilotAbility());
            }
        }
    }
}
