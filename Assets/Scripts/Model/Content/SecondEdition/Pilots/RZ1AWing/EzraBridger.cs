﻿using Mods.ModsList;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ1AWing
    {
        public class EzraBridger : RZ1AWing
        {
            public EzraBridger() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Ezra Bridger",
                    3,
                    40,
                    force: 1,
                    pilotTitle: "Spectre-6",
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.EzraBridgerPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Force, UpgradeType.Talent }
                );

                RequiredMods = new List<Type>() { typeof(PhoenixSquadronModSE) };
                ImageUrl = "https://i.imgur.com/AbHl6vG.png";
            }
        }
    }
}