using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.V19Torrent
{
    // Szajbus
    public class OddBall : V19Torrent
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

            ImageUrl = "https://files.rebel.pl/images/wydawnictwo/zapowiedzi/Star_Wars/SWZ32_odd-ball_pl.png";
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
