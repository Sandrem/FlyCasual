using Upgrade;

namespace Ship
{
    namespace SecondEdition.NantexClassStarfighter
    {
        public class Petran : NantexClassStarfighter
        {
            public Petran() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Petran???",
                    4,
                    36,
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}