using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RogueClassStarfighter
    {
        public class CadBaneSeparatist : RogueClassStarfighter
        {
            public CadBaneSeparatist() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "Cad Bane",
                    "Needs No Introduction",
                    Faction.Separatists,
                    4,
                    5,
                    18,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CadBaneSeparatistAbility),
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

                PilotNameCanonical = "cadbane-separatistalliance";

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/cadbane-separatistalliance.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CadBaneSeparatistAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}