using RuleSets;

namespace Ship
{
    namespace TIEFighter
    {
        public class ObsidianSquadronPilot: TIEFighter, ISecondEditionPilot
        {
            public ObsidianSquadronPilot() : base()
            {
                PilotName = "Obsidian Squadron Pilot";
                PilotSkill = 3;
                Cost = 13;
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 2;
                Cost = 24;
            }
        }
    }
}
