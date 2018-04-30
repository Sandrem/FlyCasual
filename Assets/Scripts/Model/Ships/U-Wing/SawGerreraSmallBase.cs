using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;
using ActionsList;
using Tokens;
using Ship;
using SubPhases;
using Upgrade;
using Mods.ModsList;

namespace Ship
{
    namespace UWing
    {
        public class SawGerreraSmallBase : UWingSmallBase
        {
            public SawGerreraSmallBase() : base()
            {
                RequiredMods.Add(typeof(UWingSmallBaseMod));

                PilotName = "Saw Gerrera (Small Base)";
                PilotSkill = 6;
                Cost = 26;

                IsUnique = true;

                PrintedUpgradeIcons.Add(UpgradeType.Elite);

                SkinName = "Partisan";

                ImageUrl = "https://i.imgur.com/DHAgMJ9.png";

                PilotAbilities.Add(new SawGerreraPilotAbility());
            }
        }
    }
}