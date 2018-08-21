using RuleSets;

namespace Ship
{
    namespace AWing
    {
        public class PrototypePilot : AWing, ISecondEditionPilot
        {
            public PrototypePilot() : base()
            {
                PilotName = "Prototype Pilot";
                PilotSkill = 1;
                Cost = 17;

                SkinName = "Blue";
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotName = "Phoenix Squadron Pilot";
                Cost = 30;
            }
        }
    }
}
