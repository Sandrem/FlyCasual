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
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.MajorVynderAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 135;
            }
        }
    }
}
