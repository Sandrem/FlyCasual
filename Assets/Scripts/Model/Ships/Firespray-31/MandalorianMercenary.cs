using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace Firespray31
    {
        public class MandalorianMercenary : Firespray31
        {
            public MandalorianMercenary() : base()
            {
                PilotName = "Mandalorian Mercenary";
                PilotSkill = 5;
                Cost = 35;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                SkinName = "Mandalorian Mercenary";

                faction = Faction.Scum;
            }
        }
    }
}
