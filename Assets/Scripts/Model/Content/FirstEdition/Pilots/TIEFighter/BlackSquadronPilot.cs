using RuleSets;

namespace Ship
{
    namespace FirstEdition.TIEFighter
    {
        public class BlackSquadronPilot : TIEFighter
        {
            public BlackSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Black Squadron Pilot",
                    4,
                    14
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}
