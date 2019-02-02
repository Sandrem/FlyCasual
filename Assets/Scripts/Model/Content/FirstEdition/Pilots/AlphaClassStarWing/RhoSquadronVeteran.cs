using Upgrade;

namespace Ship
{
    namespace FirstEdition.AlphaClassStarWing
    {
        public class RhoSquadronVeteran : AlphaClassStarWing
        {
            public RhoSquadronVeteran() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Rho Squadron Veteran",
                    4,
                    21,
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}
