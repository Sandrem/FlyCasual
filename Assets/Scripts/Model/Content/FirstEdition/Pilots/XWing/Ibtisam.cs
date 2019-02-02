﻿using System.Collections;
using System.Collections.Generic;
using Mods.ModsList;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.XWing
    {
        public class Ibtisam : XWing
        {
            public Ibtisam() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Ibtisam",
                    6,
                    26,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.IbtisamAbiliity),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                RequiredMods.Add(typeof(MyOtherRideIsMod));
                ImageUrl = "https://i.imgur.com/UteVMCP.png";
            }
        }
    }
}