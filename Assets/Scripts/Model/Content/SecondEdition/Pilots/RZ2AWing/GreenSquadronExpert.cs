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

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/3f7ad9efb4c5af8b4d1f5c07a3c7538b.png";
            }
        }
    }
}