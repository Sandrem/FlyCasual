using Upgrade;

namespace Ship
{
    namespace FirstEdition.ProtectorateStarfighter
    {
        public class ConcordDawnVeteran : ProtectorateStarfighter
        {
            public ConcordDawnVeteran() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Concord Dawn Veteran",
                    3,
                    22,
                    extraUpgradeIcon: UpgradeType.Elite
                );
            }
        }
    }
}