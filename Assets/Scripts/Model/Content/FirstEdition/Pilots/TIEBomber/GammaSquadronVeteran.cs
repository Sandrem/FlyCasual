using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEBomber
    {
        public class GammaSquadronVeteran : TIEBomber
        {
            public GammaSquadronVeteran() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Gamma Squadron Veteran",
                    5,
                    19
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);
            }
        }
    }
}
