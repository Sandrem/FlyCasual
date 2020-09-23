namespace Ship
{
    namespace SecondEdition.XiClassLightShuttle
    {
        public class FirstOrderCourier : XiClassLightShuttle
        {
            public FirstOrderCourier() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "First Order Courier",
                    2,
                    38
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/9f/49/9f490467-49a5-456f-b649-42cb74ecdd8a/swz69_a1_ship_courier.png";

                PilotNameCanonical = "firstordercourier-xiclasslightshuttle";
            }
        }
    }
}

