namespace Ship
{
    namespace SecondEdition.ModifiedTIELnFighter
    {
        public class MiningGuildSentry : ModifiedTIELnFighter
        {
            public MiningGuildSentry() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Mining Guild Sentry",
                    1,
                    24 //,
                    // seImageNumber: 92
                );

                // Ability

                ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/4/44/Swz23_mining-guild-sentry.png";
            }
        }
    }
}
