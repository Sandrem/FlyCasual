namespace Ship
{
    namespace FirstEdition.UpsilonClassShuttle
    {
        public class StarkillerBasePilot : UpsilonClassShuttle
        {
            public StarkillerBasePilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Starkiller Base Pilot",
                    2,
                    30
                );
            }
        }
    }
}
