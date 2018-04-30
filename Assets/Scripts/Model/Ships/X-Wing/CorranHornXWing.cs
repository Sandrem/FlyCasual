using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;
using Mods.ModsList;

namespace Ship
{
    namespace XWing
    {
        public class CorranHornXWing : XWing
        {
            public CorranHornXWing() : base()
            {
                PilotName = "Corran Horn";
                PilotSkill = 8;
                Cost = 28;

                ImageUrl = "https://i.imgur.com/3uwdfCc.png";

                IsUnique = true;

                SkinName = "Green";

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new CorranHornAbility());

                RequiredMods.Add(typeof(MyOtherRideIsMod));
            }
        }
    }
}