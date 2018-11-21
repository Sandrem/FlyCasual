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
                    abilityType: typeof(Abilities.FirstEdition.SerissuAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 183;
            }
        }
    }
}