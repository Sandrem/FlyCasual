using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIESfFighter
    {
        public class OmegaSpecialist : TIESfFighter
        {
            public OmegaSpecialist() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Omega Specialist",
                    5,
                    25,
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}
