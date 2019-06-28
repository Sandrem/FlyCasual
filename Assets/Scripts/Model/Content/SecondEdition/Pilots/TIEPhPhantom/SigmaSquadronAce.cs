using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEPhPhantom
    {
        public class SigmaSquadronAce : TIEPhPhantom
        {
            public SigmaSquadronAce() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Sigma Squadron Ace",
                    4,
                    48,
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 133
                );
            }
        }
    }
}
