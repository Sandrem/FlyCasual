﻿using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEBaInterceptor
    {
        public class FirstOrderProvocateur : TIEBaInterceptor
        {
            public FirstOrderProvocateur() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "First Order Provocateur",
                    "",
                    Faction.FirstOrder,
                    3,
                    4,
                    3,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Tech,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/ef/d7/efd7cc94-bdf7-4f63-80eb-476444dfeb28/swz62_card_first-order-provocateur.png";
            }
        }
    }
}
