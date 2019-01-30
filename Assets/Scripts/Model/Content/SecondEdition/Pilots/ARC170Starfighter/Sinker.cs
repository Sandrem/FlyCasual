using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ARC170Starfighter
    {
        public class Sinker : ARC170Starfighter
        {
            public Sinker() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Sinker",
                    3,
                    50,
                    isLimited: true,
                    factionOverride: Faction.Republic,
                    abilityType: typeof(Abilities.SecondEdition.SinkerAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/4e/2b/4e2bb1a3-4865-421d-898f-5272f1ab3b73/swz33_sinker.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SinkerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            // TODO
        }

        public override void DeactivateAbility()
        {
            // TODO
        }
    }
}