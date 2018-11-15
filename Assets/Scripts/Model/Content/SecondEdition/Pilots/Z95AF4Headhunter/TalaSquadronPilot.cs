using RuleSets;
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
                    25
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 29;
            }
        }
    }
}
