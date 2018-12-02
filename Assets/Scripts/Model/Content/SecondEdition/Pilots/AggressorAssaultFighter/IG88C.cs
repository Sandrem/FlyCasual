using ActionsList;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.AggressorAssaultFighter
    {
        public class IG88C : AggressorAssaultFighter
        {
            public IG88C() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "IG-88C",
                    4,
                    70,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.IG88CAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 199
                );
            }
        }
    }
}