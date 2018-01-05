using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace M12LKimogila
    {
        public class CartelExecutioner : M12LKimogila
        {
            public CartelExecutioner() : base()
            {
                PilotName = "Cartel Executioner";
                PilotSkill = 5;
                Cost = 24;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}
