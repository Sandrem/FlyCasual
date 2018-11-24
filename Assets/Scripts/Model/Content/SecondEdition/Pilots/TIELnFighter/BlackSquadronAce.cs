using RuleSets;
using Upgrade;

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
                    26,
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 90
                );
            }
        }
    }
}
