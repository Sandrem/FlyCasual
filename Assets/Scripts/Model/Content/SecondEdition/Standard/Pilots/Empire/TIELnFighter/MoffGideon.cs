using Abilities.SecondEdition;
using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class MoffGideon : TIELnFighter
        {
            public MoffGideon() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "Moff Gideon",
                    "Ruthless Remnant Leader",
                    Faction.Imperial,
                    4,
                    3,
                    8,
                    isLimited: true,
                    abilityType: typeof(MoffGideonAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Illicit,
                        UpgradeType.Illicit,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/moffgideon.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MoffGideonAbility : GenericAbility
    {
        public override void ActivateAbility()
        {

        }

        public override void DeactivateAbility()
        {

        }
    }
}