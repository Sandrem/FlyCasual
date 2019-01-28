using Actions;
using ActionsList;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.StarViperClassAttackPlatform
    {
        public class Guri : StarViperClassAttackPlatform
        {
            public Guri() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Guri",
                    5,
                    63,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.GuriAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 178
                );

                ShipInfo.ActionIcons.SwitchToDroidActions();
            }
        }
    }
}