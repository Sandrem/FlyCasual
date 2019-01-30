using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.V19Torrent
{
    // Luzak
    public class Tucker : V19Torrent
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

            ImageUrl = "https://files.rebel.pl/images/wydawnictwo/zapowiedzi/Star_Wars/SWZ32_tucker_pl.png";
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
