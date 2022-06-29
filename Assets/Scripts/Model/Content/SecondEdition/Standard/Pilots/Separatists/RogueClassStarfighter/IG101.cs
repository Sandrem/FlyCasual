using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RogueClassStarfighter
    {
        public class IG101 : RogueClassStarfighter
        {
            public IG101() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "IG-101",
                    "Tenacious Bodyguard",
                    Faction.Separatists,
                    4,
                    5,
                    19,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.IG101Ability),
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

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/ig101.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class IG101Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}