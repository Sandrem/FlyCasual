using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace AttackShuttle
    {
        public class EzraBridger : AttackShuttle
        {
            public EzraBridger() : base()
            {
                PilotName = "Ezra Bridger";
                PilotSkill = 4;
                Cost = 20;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}
