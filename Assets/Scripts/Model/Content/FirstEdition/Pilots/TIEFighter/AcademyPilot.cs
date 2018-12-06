using RuleSets;

namespace Ship
{
    namespace FirstEdition.TIEFighter
    {
        public class AcademyPilot: TIEFighter
        {
            public AcademyPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Academy Pilot",
                    1,
                    12
                );
            }
        }
    }
}
