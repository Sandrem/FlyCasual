using Editions;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class AcademyPilot: TIELnFighter
        {
            public AcademyPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Academy Pilot",
                    1,
                    22,
                    seImageNumber: 92
                );
            }
        }
    }
}
