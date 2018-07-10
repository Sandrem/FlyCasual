using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace AlphaClassStarWing
    {
        public class RhoSquadronVeteran : AlphaClassStarWing
        {
            public RhoSquadronVeteran() : base()
            {
                PilotName = "Rho Squadron Veteran";
                PilotSkill = 4;
                Cost = 21;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}
