using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mods.ModsList;

namespace Ship
{
    namespace XWing
    {
        public class HeraSyndulla : XWing
        {
            public HeraSyndulla() : base()
            {
                RequiredMods.Add(typeof(PhoenixSquadronMod));

                PilotName = "Hera Syndulla";
                PilotSkill = 7;
                Cost = 27;

                ImageUrl = "https://i.imgur.com/oBy5pDE.png";

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Green";

                PilotAbilities.Add(new Abilities.HeraSyndullaAbility());
            }
        }
    }
}
