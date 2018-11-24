using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEStriker
    {
        public class BlackSquadronScout : TIEStriker
        {
            public BlackSquadronScout() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Black Squadron Scout",
                    4,
                    20,
                    extraUpgradeIcon: UpgradeType.Elite
                );
            }
        }
    }
}
