﻿using Mods.ModsList;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.BWing
    {
        public class HeraSyndulla : BWing
        {
            public HeraSyndulla() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Hera Syndulla",
                    7,
                    29,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.HeraSyndullaAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                RequiredMods.Add(typeof(PhoenixSquadronMod));
                ImageUrl = "https://i.imgur.com/L6wpW8S.png";
            }
        }
    }
}
