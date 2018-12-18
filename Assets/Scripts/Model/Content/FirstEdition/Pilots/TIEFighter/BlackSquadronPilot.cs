using Editions;
using Upgrade;

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
                    14,
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}
