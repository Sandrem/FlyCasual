using Upgrade;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class RedSquadronVeteran : T65XWing
        {
            public RedSquadronVeteran() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Red Squadron Veteran",
                    3,
                    43,
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 10
                );
            }
        }
    }
}
