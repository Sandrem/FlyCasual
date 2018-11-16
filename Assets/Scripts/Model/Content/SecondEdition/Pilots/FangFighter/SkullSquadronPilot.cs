using Upgrade;

namespace Ship
{
    namespace SecondEdition.FangFighter
    {
        public class SkullSquadronPilot : FangFighter
        {
            public SkullSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Skull Squadron Pilot",
                    4,
                    50
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 159;
            }
        }
    }
}