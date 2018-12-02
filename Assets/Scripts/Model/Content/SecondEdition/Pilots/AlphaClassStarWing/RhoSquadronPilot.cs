using Upgrade;

namespace Ship
{
    namespace SecondEdition.AlphaClassStarWing
    {
        public class RhoSquadronPilot : AlphaClassStarWing
        {
            public RhoSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Rho Squadron Pilot",
                    3,
                    37,
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 137
                );
            }
        }
    }
}
