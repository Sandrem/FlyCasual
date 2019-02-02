using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESaBomber
    {
        public class GammaSquadronAce : TIESaBomber, TIE
        {
            public GammaSquadronAce() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Gamma Squadron Ace",
                    3,
                    32,
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 111
                );
            }
        }
    }
}
