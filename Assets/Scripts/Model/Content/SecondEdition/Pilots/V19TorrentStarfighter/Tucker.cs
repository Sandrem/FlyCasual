using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.V19TorrentStarfighter
{
    public class Tucker : V19TorrentStarfighter
    {
        public Tucker()
        {
            PilotInfo = new PilotCardInfo(
                "\"Tucker\"",
                2,
                42,
                true,
                abilityType: typeof(Abilities.SecondEdition.TuckerAbility)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/79/05/790527b0-486c-4f5e-a1cb-bab1cf29fb5b/swz32_tucker.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TuckerAbility : GenericAbility
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
