using RuleSets;

namespace Ship
{
    namespace XWing
    {
        public class BlueSquadronEscort : XWing, ISecondEditionPilot
        {
            public BlueSquadronEscort() : base()
            {
                PilotName = "Blue Squadron Escort";
                PilotSkill = 2;
                Cost = 41;
                
                PilotRuleType = typeof(SecondEdition);
            }

            public void AdaptPilotToSecondEdition()
            {
                // No Changes
            }
        }
    }
}
