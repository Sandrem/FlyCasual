using Upgrade;

namespace Ship
{
    namespace SecondEdition.Z95AF4Headhunter
    {
        public class TalaSquadronPilot : Z95AF4Headhunter
        {
            public TalaSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Tala Squadron Pilot",
                    2,
                    25,
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 29
                );
            }
        }
    }
}
