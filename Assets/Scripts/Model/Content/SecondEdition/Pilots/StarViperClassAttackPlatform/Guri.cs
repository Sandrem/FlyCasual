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
                    62,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.GuriAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 178
                );

                ShipInfo.ActionIcons.SwitchToDroidActions();
            }
        }
    }
}