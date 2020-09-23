using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.HMPDroidGunship
    {
        public class SeparatistPredator : HMPDroidGunship
        {
            public SeparatistPredator() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Separatist Predator",
                    3,
                    36,
                    extraUpgradeIcons: new List<UpgradeType> { UpgradeType.Crew, UpgradeType.Device }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/63/42/63427875-49ac-4a44-953e-ee14de2f932d/swz71_card_predator.png";
            }

            
        }
    }
}