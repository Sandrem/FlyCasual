using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ2AWing
    {
        public class GreenSquadronExpert : RZ2AWing
        {
            public GreenSquadronExpert() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Green Squadron Expert",
                    3,
                    34,
                    extraUpgradeIcon: UpgradeType.Talent //,
                                                         //seImageNumber: 19
                );

                ModelInfo.SkinName = "Green";

                ImageUrl = "http://infinitearenas.com/xw2browse/images/resistance/green-squadron-expert.jpg";
            }
        }
    }
}