using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.V19Torrent
{
    // Topor
    public class Axe : V19Torrent
    {
        public Axe()
        {
            PilotInfo = new PilotCardInfo(
                "\"Axe\"",
                3,
                44,
                true,
                abilityType: typeof(Abilities.SecondEdition.AxeAbility),
                extraUpgradeIcon: UpgradeType.Talent
            );

            ImageUrl = "https://files.rebel.pl/images/wydawnictwo/zapowiedzi/Star_Wars/SWZ32_axe_PL.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AxeAbility : GenericAbility
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
