using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESeBomber
    {
        public class FirstOrderCadet : TIESeBomber
        {
            public FirstOrderCadet() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "First Order Cadet",
                    3,
                    32,
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ImageUrl = "https://i.imgur.com/VWm0SbB.png";
            }
        }
    }
}
