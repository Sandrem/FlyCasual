namespace Ship
{
    namespace SecondEdition.M3AInterceptor
    {
        public class CartelSpacer : M3AInterceptor
        {
            public CartelSpacer() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Cartel Spacer",
                    1,
                    29
                );

                SEImageNumber = 190;
            }
        }
    }
}