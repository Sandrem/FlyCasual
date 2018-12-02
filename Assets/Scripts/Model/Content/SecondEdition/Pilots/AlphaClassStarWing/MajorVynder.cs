using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.AlphaClassStarWing
    {
        public class MajorVynder : AlphaClassStarWing
        {
            public MajorVynder() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Major Vynder",
                    4,
                    41,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.MajorVynderAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 135
                );
            }
        }
    }
}
