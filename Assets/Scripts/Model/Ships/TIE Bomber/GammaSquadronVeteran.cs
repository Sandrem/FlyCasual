using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEBomber
    {
        public class GammaSquadronVeteran : TIEBomber
        {
            public GammaSquadronVeteran() : base()
            {
                PilotName = "Gamma Squadron Veteran";
                PilotSkill = 5;
                Cost = 19;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "White Stripes";
            }
        }
    }
}
