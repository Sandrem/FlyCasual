using RuleSets;

namespace Ship
{
    namespace TIEFighter
    {
        public class AcademyPilot: TIEFighter, ISecondEditionPilot
        {
            public AcademyPilot() : base()
            {
                PilotName = "Academy Pilot";
                PilotSkill = 1;
                Cost = 12;
            }

            public void AdaptPilotToSecondEdition()
            {
                ImageUrl = "https://i.imgur.com/gzavoA8.png";

                Cost = 24;
            }
        }
    }
}
