using System;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class AmaxineWarrior : BTLA4YWing
        {
            public AmaxineWarrior() : base()
            {
                RequiredMods = new List<Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

                PilotInfo = new PilotCardInfo
                (
                    "Amaxine Warrior",
                    3,
                    33,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Tech,
                        UpgradeType.Illicit
                    },
                    factionOverride: Faction.Scum
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/52/7d/527dd63a-1c64-4d78-bac3-b7b999accaf9/swz85_pilot_amaxinewarrior.png";
            }
        }
    }
}
