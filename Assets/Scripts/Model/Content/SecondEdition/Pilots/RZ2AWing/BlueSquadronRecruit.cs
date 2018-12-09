using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ2AWing
    {
        public class BlueSquadronRecruit : RZ2AWing
        {
            public BlueSquadronRecruit() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Blue Squadron Recruit",
                    1,
                    32
                    //extraUpgradeIcon: UpgradeType.Talent //,
                                                         //seImageNumber: 19
                );

                ModelInfo.SkinName = "Blue";

                ImageUrl = "http://infinitearenas.com/xw2browse/images/resistance/blue-squadron-recruit.jpg";
            }
        }
    }
}