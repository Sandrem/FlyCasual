using Upgrade;

namespace Ship
{
    namespace SecondEdition.NantexClassStarfighter
    {
        public class StalgasinHiveGuard : NantexClassStarfighter
        {
            public StalgasinHiveGuard() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Stalgasin Hive Guard",
                    3,
                    34
                );

                ImageUrl = "https://i.imgur.com/TnkBzUW.png";
            }
        }
    }
}