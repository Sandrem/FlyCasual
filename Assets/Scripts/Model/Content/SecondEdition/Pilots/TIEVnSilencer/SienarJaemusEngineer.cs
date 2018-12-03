using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEVnSilencer
    {
        public class SienarJaemusEngineer : TIEVnSilencer
        {
            public SienarJaemusEngineer() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Sienar-Jaemus Engineer",
                    1,
                    56 //,
                    //seImageNumber: 120
                );

                ImageUrl = "http://www.infinitearenas.com/xw2browse/images/first-order/sienar-jaemus-engineer.jpg";
            }
        }
    }
}
