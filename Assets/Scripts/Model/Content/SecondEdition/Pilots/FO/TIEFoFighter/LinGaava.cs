using Content;
using Ship;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEFoFighter
    {
        public class LinGaava : TIEFoFighter
        {
            public LinGaava() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "Lin Gaava",
                    "Impetuous Mechanic",
                    Faction.FirstOrder,
                    3,
                    3,
                    9,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LinGaavaAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Tech,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://i.imgur.com/lwIAssA.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LinGaavaAbility : GenericAbility
    {
        public override void ActivateAbility()
        {

        }

        public override void DeactivateAbility()
        {

        }
    }
}
