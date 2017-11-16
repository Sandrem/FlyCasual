using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace SheathipedeShuttle
    {
        public class FennRau : SheathipedeShuttle
        {
            public FennRau() : base()
            {
                PilotName = "Fenn Rau";
                PilotSkill = 9;
                Cost = 20;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}
