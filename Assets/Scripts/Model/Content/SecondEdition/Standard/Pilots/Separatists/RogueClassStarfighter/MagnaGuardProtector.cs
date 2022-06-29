using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RogueClassStarfighter
    {
        public class MagnaGuardProtector : RogueClassStarfighter
        {
            public MagnaGuardProtector() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "MagnaGuard Protector",
                    "Implacable Escort",
                    Faction.Separatists,
                    4,
                    5,
                    18,
                    limited: 2,
                    abilityType: typeof(Abilities.SecondEdition.MagnaGuardProtectorAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Cannon,
                        UpgradeType.Cannon,
                        UpgradeType.Missile,
                        UpgradeType.Modification,
                    },
                    tags: new List<Tags>()
                    {
                        Tags.Droid
                    }
                );

                ShipInfo.ActionIcons.SwitchToDroidActions();

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/magnaguardprotector.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MagnaGuardProtectorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}