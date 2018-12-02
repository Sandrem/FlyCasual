using RuleSets;

namespace Ship
{
    namespace FirstEdition.TIEFighter
    {
        public class ObsidianSquadronPilot : TIEFighter
        {
            public ObsidianSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Obsidian Squadron Pilot",
                    3,
                    13
                );
            }
        }
    }
}
