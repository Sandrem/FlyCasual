using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.CloneZ95Headhunter
    {
        public class Killer : CloneZ95Headhunter
        {
            public Killer() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "\"Killer\"",
                    "Dependable Closer",
                    Faction.Republic,
                    2,
                    3,
                    11,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.KillerAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Cannon,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Clone
                    }
                );

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/killer.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KillerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {

        }

        public override void DeactivateAbility()
        {

        }
    }
}