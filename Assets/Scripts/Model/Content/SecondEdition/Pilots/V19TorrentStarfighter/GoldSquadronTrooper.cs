using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.V19TorrentStarfighter
{
    public class GoldSquadronTrooper : V19TorrentStarfighter
    {
        public GoldSquadronTrooper()
        {
            PilotInfo = new PilotCardInfo(
                "Gold Squadron Trooper",
                2,
                25
            );
            
            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/ab/a7/aba7c941-3472-453f-bfd8-032f5f854e0b/swz32_gold-sqd-trooper.png";
        }
    }
}