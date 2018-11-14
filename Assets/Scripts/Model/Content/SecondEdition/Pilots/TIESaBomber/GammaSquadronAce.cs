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
                    30
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                ModelInfo.SkinName = "White Stripes";

                SEImageNumber = 111;
            }
        }
    }
}
