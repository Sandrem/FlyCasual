namespace Ship
{
    namespace SecondEdition.TIESkStriker
    {
        public class PlanetarySentinel : TIESkStriker
        {
            public PlanetarySentinel() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Planetary Sentinel",
                    1,
                    34,
                    seImageNumber: 121
                );
            }
        }
    }
}
