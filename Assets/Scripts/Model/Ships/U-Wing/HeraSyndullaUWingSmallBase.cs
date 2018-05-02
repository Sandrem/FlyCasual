using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mods.ModsList;
using Abilities;

namespace Ship
{
    namespace UWing
    {
        public class HeraSyndullaUWingSmallBase : UWingSmallBase
        {
            public HeraSyndullaUWingSmallBase() : base()
            {
                RequiredMods.Add(typeof(PhoenixSquadronMod));
                RequiredMods.Add(typeof(UWingSmallBaseMod));

                PilotName = "Hera Syndulla (Small Base)";
                PilotSkill = 7;
                Cost = 28;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                ImageUrl = "https://i.imgur.com/gcK261Z.png";

                PilotAbilities.Add(new HeraSyndullaAbility());
            }
        }
    }
}
