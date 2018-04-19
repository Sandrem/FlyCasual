using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mods.ModsList;
using Abilities;

namespace Ship
{
    namespace UWing
    {
        public class HeraSyndullaUWing : UWing
        {
            public HeraSyndullaUWing() : base()
            {
                RequiredMods.Add(typeof(PhoenixSquadronMod));

                PilotName = "Hera Syndulla";
                PilotSkill = 7;
                Cost = 28;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                ImageUrl = "https://i.imgur.com/zyN4zfB.png";

                PilotAbilities.Add(new HeraSyndullaAbility());
            }
        }
    }
}
