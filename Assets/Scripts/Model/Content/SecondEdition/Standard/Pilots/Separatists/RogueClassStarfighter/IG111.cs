using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RogueClassStarfighter
    {
        public class IG111 : RogueClassStarfighter
        {
            public IG111() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "IG-111",
                    "One Eye",
                    Faction.Separatists,
                    1,
                    5,
                    21,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.IG111Ability),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Cannon,
                        UpgradeType.Cannon,
                        UpgradeType.Modification,
                        UpgradeType.Modification,
                    },
                    tags: new List<Tags>()
                    {
                        Tags.Droid
                    }
                );

                ShipInfo.ActionIcons.SwitchToDroidActions();

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/ig111.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class IG111Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}