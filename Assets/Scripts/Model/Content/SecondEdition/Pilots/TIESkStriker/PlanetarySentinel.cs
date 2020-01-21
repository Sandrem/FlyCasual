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
                    31,
                    seImageNumber: 121
                );
            }
        }
    }
}
