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
                    26,
                    seImageNumber: 130
                );
            }
        }
    }
}
