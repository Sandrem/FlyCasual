using Ship;
using SubPhases;
using System.Collections.Generic;
using Upgrade;
using Tokens;
using BoardTools;
using UnityEngine;
using Content;

namespace Ship.SecondEdition.Delta7Aethersprite
{
    public class AdiGallia : Delta7Aethersprite
    {
        public AdiGallia()
        {
            IsWIP = true;

            PilotInfo = new PilotCardInfo25
            (
                "Adi Gallia",
                "Shooting Star",
                Faction.Republic,
                5,
                5,
                12,
                isLimited: true,
                force: 2,
                abilityType: typeof(Abilities.SecondEdition.AdiGalliaAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent
                },
                tags: new List<Tags>
                {
                    Tags.Jedi,
                    Tags.LightSide
                },
                skinName: "Plo Koon"
            );

            ImageUrl = "https://infinitearenas.com/xw2/images/pilots/adigallia.png";
        }
    }
}

namespace Abilities.SecondEdition
{

    public class AdiGalliaAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}
