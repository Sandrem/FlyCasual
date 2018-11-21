using Upgrade;

namespace Ship
{
    namespace FirstEdition.AlphaClassStarWing
    {
        public class RhoSquadronPilot : AlphaClassStarWing
        {
            public RhoSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Rho Squadron Pilot",
                    4,
                    21
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);
            }
        }
    }
}
