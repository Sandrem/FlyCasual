using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEAdvanced
    {
        public class MaarekStele : TIEAdvanced
        {
            public MaarekStele() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Maarek Stele",
                    7,
                    27,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.MaarekSteleAbility),
                    extraUpgradeIcon: UpgradeType.Elite
                );
            }
        }
    }
}