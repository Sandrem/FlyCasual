using Upgrade;

namespace Ship
{
    namespace FirstEdition.ProtectorateStarfighter
    {
        public class ConcordDawnAce : ProtectorateStarfighter
        {
            public ConcordDawnAce() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Concord Dawn Ace",
                    5,
                    23
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);
            }
        }
    }
}