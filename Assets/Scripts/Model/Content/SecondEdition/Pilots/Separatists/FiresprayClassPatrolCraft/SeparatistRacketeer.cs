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
                PilotInfo = new PilotCardInfo25
                (
                    "Separatist Racketeer",
                    "",
                    Faction.Separatists,
                    2,
                    7,
                    10,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Cannon,
                        UpgradeType.Missile,
                        UpgradeType.Device
                    },
                    skinName: "Jango Fett"
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/16/8c/168ca8f4-0015-44a3-9a7c-099caff70881/swz82_a1_separatist-racketeer.png";
            }
        }
    }
}
