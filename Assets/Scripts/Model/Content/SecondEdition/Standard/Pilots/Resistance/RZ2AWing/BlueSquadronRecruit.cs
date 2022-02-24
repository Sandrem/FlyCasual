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
                    33,
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ModelInfo.SkinName = "Blue";

                ImageUrl = "https://squadbuilder.fantasyflightgames.com/card_images/en/e033b2729305ac0b678d6031ada7b2b8.png";
            }
        }
    }
}