using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.CloneZ95Headhunter
    {
        public class Drift : CloneZ95Headhunter
        {
            public Drift() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "\"Drift\"",
                    "CT-1020",
                    Faction.Republic,
                    3,
                    3,
                    5,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DriftAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Clone
                    }
                );

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/drift.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DriftAbility : GenericAbility
    {
        public override void ActivateAbility()
        {

        }

        public override void DeactivateAbility()
        {

        }
    }
}