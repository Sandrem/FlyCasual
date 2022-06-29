using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.CloneZ95Headhunter
    {
        public class Knack : CloneZ95Headhunter
        {
            public Knack() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "\"Knack\"",
                    "Incautious Instructor",
                    Faction.Republic,
                    5,
                    3,
                    7,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.KnackAbility),
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

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/knack.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KnackAbility : GenericAbility
    {
        public override void ActivateAbility()
        {

        }

        public override void DeactivateAbility()
        {

        }
    }
}