using Actions;
using ActionsList;
using UnityEngine;
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
                    abilityType: typeof(Abilities.SecondEdition.GuriAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 178
                );

                ShipInfo.ActionIcons.SwitchToDroidActions();
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GuriAbility : Abilities.FirstEdition.GuriAbility
    {
        protected override Vector2 AbilityRange
        {
            get { return new Vector2(0, 1); }
        }
    }
}