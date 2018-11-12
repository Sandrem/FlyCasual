using RuleSets;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class BlackSquadronAce : TIELnFighter
        {
            public BlackSquadronAce() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Black Squadron Ace",
                    3,
                    26
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                SEImageNumber = 90;
            }
        }
    }
}
