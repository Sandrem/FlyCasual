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

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/e033b2729305ac0b678d6031ada7b2b8.png";
            }
        }
    }
}