using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.CloneZ95Headhunter
    {
        public class Boost : CloneZ95Headhunter
        {
            public Boost() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "\"Boost\"",
                    "CT-4860",
                    Faction.Republic,
                    3,
                    3,
                    8,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.BoostAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Clone
                    }
                );

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/boost.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BoostAbility : GenericAbility
    {
        public override void ActivateAbility()
        {

        }

        public override void DeactivateAbility()
        {

        }
    }
}