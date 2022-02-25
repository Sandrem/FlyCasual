using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESeBomber
    {
        public class Grudge : TIESeBomber
        {
            public Grudge() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "\"Grudge\"",
                    "Hateful Harrier",
                    Faction.FirstOrder,
                    2,
                    4,
                    15,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.GrudgePilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Gunner,
                        UpgradeType.Device
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://i.imgur.com/f24aFJJ.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GrudgePilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}
