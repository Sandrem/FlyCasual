using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.V19Torrent
{
    //Wykop
    public class Kickback : V19Torrent
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

            ImageUrl = "https://files.rebel.pl/images/wydawnictwo/zapowiedzi/Star_Wars/SWZ32_kickback_pl.png";
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
