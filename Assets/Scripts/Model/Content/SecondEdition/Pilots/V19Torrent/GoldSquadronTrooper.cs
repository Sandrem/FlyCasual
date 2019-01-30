using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.V19Torrent
{
    // Zolnierz Eskadry Zlotych
    public class GoldSquadronTrooper : V19Torrent
    {
        public GoldSquadronTrooper()
        {
            PilotInfo = new PilotCardInfo(
                "Gold Squadron Trooper",
                2,
                40
            );

            ImageUrl = "https://files.rebel.pl/images/wydawnictwo/zapowiedzi/Star_Wars/SWZ32_Gold-Sqd-trooper_pl.png";
        }
    }
}