namespace Ship.SecondEdition.VultureClassDroidFighter
{
    public class TradeFederationDrone : VultureClassDroidFighter
    {
        public TradeFederationDrone()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Trade Federation Drone",
                "",
                Faction.Separatists,
                1,
                2,
                0
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/f0/05/f005d66c-754c-4bff-8ca2-45ea67e2d074/swz31_trade-federation-drone.png";
        }
    }
}