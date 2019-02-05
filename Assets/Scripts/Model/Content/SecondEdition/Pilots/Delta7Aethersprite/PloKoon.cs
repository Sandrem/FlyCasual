using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.Delta7Aethersprite
{
    public class PloKoon : Delta7Aethersprite
    {
        public PloKoon()
        {
            PilotInfo = new PilotCardInfo(
                "Plo Koon",
                5,
                63,
                true,
                force: 2,
                abilityType: typeof(Abilities.SecondEdition.PloKoonAbility),
                extraUpgradeIcon: UpgradeType.Force
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/6a/6f/6a6fef51-fb5f-49c1-b5cc-8e96b6d09051/swz32_plo-koon.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PloKoonAbility : GenericAbility
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
