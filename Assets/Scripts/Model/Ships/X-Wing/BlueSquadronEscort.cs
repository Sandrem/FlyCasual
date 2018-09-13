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

                SEImageNumber = 11;
            }

            public void AdaptPilotToSecondEdition()
            {
                // No Changes
            }
        }
    }
}
