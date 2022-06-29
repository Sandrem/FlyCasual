using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RogueClassStarfighter
    {
        public class CadBaneScum : RogueClassStarfighter
        {
            public CadBaneScum() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "Cad Bane",
                    "Infamous Bounty Hunter",
                    Faction.Scum,
                    4,
                    5,
                    18,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CadBaneScumAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Cannon,
                        UpgradeType.Cannon,
                        UpgradeType.Missile,
                        UpgradeType.Illicit,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    tags: new List<Tags>()
                    {
                        Tags.BountyHunter
                    }
                );

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/cadbane.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CadBaneScumAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}