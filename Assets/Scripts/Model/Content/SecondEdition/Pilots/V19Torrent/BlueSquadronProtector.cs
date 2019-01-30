using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.V19Torrent
{
    // Protektor Eskadry Niebieskich
    public class BlueSquadronProtector : V19Torrent
    {
        public BlueSquadronProtector()
        {
            PilotInfo = new PilotCardInfo(
                "Blue Squadron Protector",
                3,
                43,
                extraUpgradeIcon: UpgradeType.Talent
            );

            ImageUrl = "https://files.rebel.pl/images/wydawnictwo/zapowiedzi/Star_Wars/SWZ32_Blue-Sqd-protector_pl.png";
        }
    }
}