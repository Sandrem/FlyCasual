using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;
using Ship;

namespace Ship
{
    namespace T70XWing
    {
        public class PoeDameronHotr : T70XWing
        {
            public PoeDameronHotr() : base()
            {
                PilotName = "Poe Dameron (HotR)";
                PilotSkill = 9;
                Cost = 33;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Black One";

                PilotAbilities.Add(new PoeDameronAbility());
            }
        }
    }
}