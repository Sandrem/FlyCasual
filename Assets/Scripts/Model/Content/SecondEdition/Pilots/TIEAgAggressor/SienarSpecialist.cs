namespace Ship
{
    namespace SecondEdition.TIEAgAggressor
    {
        public class SienarSpecialist : TIEAgAggressor, TIE
        {
            public SienarSpecialist() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Sienar Specialist",
                    2,
                    28,
                    seImageNumber: 130
                );
            }
        }
    }
}
