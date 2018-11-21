using Upgrade;

namespace Ship
{
    namespace SecondEdition.UT60DUWing
    {
        public class BenthicTwoTubes : UT60DUWing
        {
            public BenthicTwoTubes() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Benthic Two Tubes",
                    2,
                    47,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.BenthicTwoTubesAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Illicit);

                ModelInfo.SkinName = "Partisan";

                SEImageNumber = 58;
            }
        }
    }
}