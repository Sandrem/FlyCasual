using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEReaper
    {
        public class MajorVermeil : TIEReaper
        {
            public MajorVermeil() : base()
            {
                PilotName = "Major Vermeil";
                PilotSkill = 6;
                Cost = 26;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                ImageUrl = "https://i.imgur.com/vBp2GWs.jpg";
            }
        }
    }
}
