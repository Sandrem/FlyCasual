using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.Delta7Aethersprite
{
    public class ObiWanKenobi : Delta7Aethersprite
    {
        public ObiWanKenobi()
        {
            PilotInfo = new PilotCardInfo(
                "Obi-Wan Kenobi",
                5,
                65,
                true,
                force: 3,
                abilityType: typeof(Abilities.SecondEdition.ObiWanKenobiAbility),
                extraUpgradeIcon: UpgradeType.Force
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/f9/24/f9246e39-4852-4a8f-a331-9b78f62439e9/swz32_obi-wan-kenobi.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ObiWanKenobiAbility : GenericAbility
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
