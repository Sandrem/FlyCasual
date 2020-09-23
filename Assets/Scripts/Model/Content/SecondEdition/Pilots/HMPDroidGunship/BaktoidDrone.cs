using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.HMPDroidGunship
    {
        public class BaktoidDrone : HMPDroidGunship
        {
            public BaktoidDrone() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Baktoid Drone",
                    1,
                    34,
                    extraUpgradeIcons: new List<UpgradeType> { UpgradeType.Crew, UpgradeType.Device }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/7c/61/7c61a6d9-0c5a-4bc0-9d4d-f38e3723a2c1/swz71_card_baktoid.png";
            }

            
        }
    }
}