using Upgrade;

namespace Ship
{
    namespace SecondEdition.LancerClassPursuitCraft
    {
        public class SabineWren : LancerClassPursuitCraft
        {
            public SabineWren() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Sabine Wren",
                    3,
                    68,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.SabineWrenLancerPilotAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 220
                );
            }
        }
    }
}
