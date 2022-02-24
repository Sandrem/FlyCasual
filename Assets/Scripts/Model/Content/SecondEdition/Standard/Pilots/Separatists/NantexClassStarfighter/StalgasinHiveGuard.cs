namespace Ship
{
    namespace SecondEdition.NantexClassStarfighter
    {
        public class StalgasinHiveGuard : NantexClassStarfighter
        {
            public StalgasinHiveGuard() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Stalgasin Hive Guard",
                    "",
                    Faction.Separatists,
                    3,
                    4,
                    4
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/02/20/02205574-9881-46ff-99a1-a74ad5bb0137/swz47_cards-hive-guard.png";
            }
        }
    }
}