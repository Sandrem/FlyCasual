using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEAdvancedX1
    {
        public class MaarekStele : TIEAdvancedX1
        {
            public MaarekStele() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Maarek Stele",
                    5,
                    46,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.MaarekSteleAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 94
                );
            }
        }
    }
}
