using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RogueClassStarfighter
    {
        public class IG102 : RogueClassStarfighter
        {
            public IG102() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "IG-102",
                    "Dueling Droid",
                    Faction.Separatists,
                    4,
                    5,
                    19,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.IG102Ability),
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

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/ig102.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class IG102Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}