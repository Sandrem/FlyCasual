using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.V19Torrent
{
    // Nalot
    public class Swoop : V19Torrent
    {
        public Swoop()
        {
            PilotInfo = new PilotCardInfo(
                "\"Swoop\"",
                3,
                44,
                true,
                abilityType: typeof(Abilities.SecondEdition.SwoopAbility),
                extraUpgradeIcon: UpgradeType.Talent
            );

            ImageUrl = "https://files.rebel.pl/images/wydawnictwo/zapowiedzi/Star_Wars/SWZ32_swoop_pl.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SwoopAbility : GenericAbility
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
