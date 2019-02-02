using Upgrade;

namespace Ship
{
    namespace SecondEdition.M3AInterceptor
    {
        public class Serissu : M3AInterceptor
        {
            public Serissu() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Serissu",
                    5,
                    40,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.SerissuAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 183
                );
            }
        }
    }
}