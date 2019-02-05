using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.V19TorrentStarfighter
{
    public class Swoop : V19TorrentStarfighter
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

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/c3/9f/c39f4623-a983-4fea-98aa-c11b37e867c0/swz32_swoop.png";
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
