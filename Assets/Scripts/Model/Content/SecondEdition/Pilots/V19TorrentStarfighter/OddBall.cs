using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.V19TorrentStarfighter
{
    public class OddBall : V19TorrentStarfighter
    {
        public OddBall()
        {
            PilotInfo = new PilotCardInfo(
                "\"Odd Ball\"",
                5,
                48,
                true,
                abilityType: typeof(Abilities.SecondEdition.OddBallAbility),
                extraUpgradeIcon: UpgradeType.Talent
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/18/d2/18d2b2c2-482a-4b6f-8c53-c3f0f24bea4b/swz32_odd-ball.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class OddBallAbility : GenericAbility
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
