using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FiresprayClassPatrolCraft
    {
        public class SeparatistRacketeer : FiresprayClassPatrolCraft
        {
            public SeparatistRacketeer() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Separatist Racketeer",
                    2,
                    64,
                    factionOverride: Faction.Separatists,
                    extraUpgradeIcon: UpgradeType.Crew
                );

                ModelInfo.SkinName = "Jango Fett";

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/16/8c/168ca8f4-0015-44a3-9a7c-099caff70881/swz82_a1_separatist-racketeer.png";
            }
        }
    }
}
