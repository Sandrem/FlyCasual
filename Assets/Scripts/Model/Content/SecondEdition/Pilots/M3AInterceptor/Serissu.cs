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
                    43,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.SerissuAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 183
                );
            }
        }
    }
}