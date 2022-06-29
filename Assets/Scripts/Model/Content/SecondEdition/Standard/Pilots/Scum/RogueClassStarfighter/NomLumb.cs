using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RogueClassStarfighter
    {
        public class NomLumb : RogueClassStarfighter
        {
            public NomLumb() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "Nom Lumb",
                    "Laughing Bandit",
                    Faction.Scum,
                    1,
                    5,
                    19,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.NomLumbRogueAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Cannon,
                        UpgradeType.Cannon,
                        UpgradeType.Missile,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>()
                    {
                        Tags.BountyHunter
                    }
                );

                PilotNameCanonical = "nomlumb-rogueclassstarfighter";

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/nomlumb-rogueclassstarfighter.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class NomLumbRogueAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}