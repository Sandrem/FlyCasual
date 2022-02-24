namespace Ship
{
    namespace SecondEdition.M3AInterceptor
    {
        public class CartelSpacer : M3AInterceptor
        {
            public CartelSpacer() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Cartel Spacer",
                    "",
                    Faction.Scum,
                    1,
                    4,
                    6,
                    seImageNumber: 190
                );
            }
        }
    }
}