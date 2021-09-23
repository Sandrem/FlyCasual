using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLBYWing
    {
        public class RedSquadronBomber : BTLBYWing
        {
            public RedSquadronBomber() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Red Squadron Bomber",
                    2,
                    30,
                    extraUpgradeIcon: UpgradeType.Astromech
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/45/9f/459f895c-ba35-4d01-819b-653c2e4b7b96/swz48_pilot-red-sqd-bomber.png";
            }
        }
    }
}
