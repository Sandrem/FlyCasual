using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.V19TorrentStarfighter
{
    public class Axe : V19TorrentStarfighter
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

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/2c/ee/2ceea646-b5bd-42ce-aeb1-7f38dc88e045/swz32_axe.png";
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
