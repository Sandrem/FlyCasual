using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEInterceptor
    {
        public class RoyalGuardPilot : TIEInterceptor
        {
            public RoyalGuardPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Royal Guard Pilot",
                    6,
                    22
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                ModelInfo.SkinName = "Royal Guard";
            }
        }
    }
}