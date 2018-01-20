using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEStriker
    {
        public class BlackSquadronScout : TIEStriker
        {
            public BlackSquadronScout() : base()
            {
                PilotName = "Black Squadron Scout";
                PilotSkill = 4;
                Cost = 20;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}
