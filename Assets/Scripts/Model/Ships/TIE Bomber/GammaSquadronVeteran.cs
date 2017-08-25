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
                ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/b/bd/Swx52-gamma-squad-vet.png";
                PilotSkill = 5;
                Cost = 19;

                AddUpgradeSlot(Upgrade.UpgradeType.Elite);
            }
        }
    }
}
