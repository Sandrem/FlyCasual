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
                    29
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/02/20/02205574-9881-46ff-99a1-a74ad5bb0137/swz47_cards-hive-guard.png";
            }
        }
    }
}