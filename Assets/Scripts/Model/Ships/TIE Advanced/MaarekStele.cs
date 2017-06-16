using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEAdvanced
    {
        public class MaarekStele : TIEAdvanced
        {
            public MaarekStele() : base()
            {
                PilotName = "Maarek Stele";
                ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/4/41/Maarek_Stele.png";
                IsUnique = true;
                PilotSkill = 7;
                Cost = 27;
                AddUpgradeSlot(Upgrade.UpgradeSlot.Elite);
            }

            public override void InitializePilot()
            {
                base.InitializePilot();
                // TODO:
                // On Crit Card dealt - selection
            }
        }
    }
}
