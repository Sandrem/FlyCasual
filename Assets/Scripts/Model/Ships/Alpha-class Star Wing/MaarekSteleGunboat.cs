using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using DamageDeckCard;
using Mods.ModsList;

namespace Ship
{
    namespace AlphaClassStarWing
    {
        public class MaarekSteleGunboat : AlphaClassStarWing
        {
            public MaarekSteleGunboat() : base()
            {
                PilotName = "Maarek Stele";
                PilotSkill = 7;
                Cost = 27;

                ImageUrl = "https://i.imgur.com/SFGZXbc.png";

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.MaarekSteleAbility());

                SkinName = "Red";

                RequiredMods.Add(typeof(MyOtherRideIsMod));
            }
        }
    }
}