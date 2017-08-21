using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEInterceptor
    {
        public class SoontirFell : TIEInterceptor
        {
            public SoontirFell() : base()
            {
                PilotName = "Soontir Fell";
                ImageUrl = "https://vignette4.wikia.nocookie.net/xwing-miniatures/images/c/c2/Alpha_Squadron_Pilot.png";
                PilotSkill = 9;
                Cost = 27;

                IsUnique = true;
                AddUpgradeSlot(Upgrade.UpgradeSlot.Elite);
            }
        }
    }
}
