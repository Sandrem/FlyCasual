using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTANR2YWing
    {
        public class NewRepublicPatrol : BTANR2YWing
        {
            public NewRepublicPatrol() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "New Republic Patrol",
                    3,
                    32,
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ImageUrl = "https://i.imgur.com/7zFI7ZH.png";
            }
        }
    }
}
