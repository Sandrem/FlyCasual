namespace Ship.SecondEdition.SithInfiltrator
{
    public class DarkCourier : SithInfiltrator
    {
        public DarkCourier()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Dark Courier",
                "",
                Faction.Separatists,
                2,
                6,
                9
            );
            
            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/e2/02/e20245ab-47e9-41f9-abdf-62f571246faf/swz30_dark-courier.png";
        }
    }
}