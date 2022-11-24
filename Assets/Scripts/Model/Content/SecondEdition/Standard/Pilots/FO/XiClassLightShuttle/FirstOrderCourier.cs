using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.XiClassLightShuttle
    {
        public class FirstOrderCourier : XiClassLightShuttle
        {
            public FirstOrderCourier() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "First Order Courier",
                    "",
                    Faction.FirstOrder,
                    2,
                    4,
                    10,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Tech,
                        UpgradeType.Tech,
                        UpgradeType.Crew,
                        UpgradeType.Modification
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/9f/49/9f490467-49a5-456f-b649-42cb74ecdd8a/swz69_a1_ship_courier.png";

                PilotNameCanonical = "firstordercourier-xiclasslightshuttle";
            }
        }
    }
}

