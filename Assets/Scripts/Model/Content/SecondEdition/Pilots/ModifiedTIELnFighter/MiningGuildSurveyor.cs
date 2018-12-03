using Upgrade;

namespace Ship
{
    namespace SecondEdition.ModifiedTIELnFighter
    {
        public class MiningGuildSurveyor : ModifiedTIELnFighter
        {
            public MiningGuildSurveyor() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Mining Guild Surveyor",
                    2,
                    25,
                    extraUpgradeIcon: UpgradeType.Talent //,
                    // seImageNumber: 92
                );

                // Ability

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/bf/da/bfda499f-603a-41c7-b2ee-50ffeeddb384/swz23_mining-guild-surveyor.png";
            }
        }
    }
}
