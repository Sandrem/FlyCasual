using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mods.ModsList;

namespace Ship
{
    namespace BWing
    {
        public class HeraSyndullaBWing : BWing
        {
            public HeraSyndullaBWing() : base()
            {
                RequiredMods.Add(typeof(PhoenixSquadronMod));

                PilotName = "Hera Syndulla";
                PilotSkill = 7;
                Cost = 29;

                ImageUrl = "https://i.imgur.com/L6wpW8S.png";

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.HeraSyndullaAbility());
            }
        }
    }
}
