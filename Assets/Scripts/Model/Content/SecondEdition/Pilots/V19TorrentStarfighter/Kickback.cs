using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.V19TorrentStarfighter
{
    public class Kickback : V19TorrentStarfighter
    {
        public Kickback()
        {
            PilotInfo = new PilotCardInfo(
                "\"Kickback\"",
                4,
                46,
                true,
                abilityType: typeof(Abilities.SecondEdition.KickbackAbility),
                extraUpgradeIcon: UpgradeType.Talent
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/57/c4/57c43689-5d1f-4fd2-b1f6-d4bec9448634/swz32_kickback.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KickbackAbility : GenericAbility
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
