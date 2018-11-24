using Upgrade;

namespace Ship
{
    namespace SecondEdition.ARC170Starfighter
    {
        public class SharaBey : ARC170Starfighter
        {
            public SharaBey() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Shara Bey",
                    4,
                    53,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.SharaBeyAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 67
                );
            }
        }
    }
}