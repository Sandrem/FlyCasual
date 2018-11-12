using RuleSets;

namespace Ship
{
    namespace SecondEdition.XWing
    {
        public class RedSquadronVeteran : XWing
        {
            public RedSquadronVeteran() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Red Squadron Veteran",
                    3,
                    43
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                SEImageNumber = 10;
            }
        }
    }
}
