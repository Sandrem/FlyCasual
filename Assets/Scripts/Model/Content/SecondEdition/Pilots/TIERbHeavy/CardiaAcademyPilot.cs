using Arcs;
using Movement;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIERbHeavy
    {
        public class CardiaAcademyPilot : TIERbHeavy
        {
            public CardiaAcademyPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Cardia Academy Pilot",
                    1,
                    30
                );
            }
        }
    }
}