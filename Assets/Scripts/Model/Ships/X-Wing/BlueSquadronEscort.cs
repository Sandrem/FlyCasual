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
                Cost = 42; //TODO
                
                PilotRuleType = typeof(SecondEdition);

                faction = Faction.Rebel;
            }

            public void AdaptPilotToSecondEdition()
            {
                // No Changes
            }
        }
    }
}
