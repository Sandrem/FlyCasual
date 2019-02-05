using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.V19Torrent
{
    public class BlueSquadronProtector : V19TorrentStarfighter
    {
        public BlueSquadronProtector()
        {
            PilotInfo = new PilotCardInfo(
                "Blue Squadron Protector",
                3,
                43,
                extraUpgradeIcon: UpgradeType.Talent
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/51/18/5118d916-b09f-47a1-a4dd-0df667267d1b/swz32_blue-sqd-protector.png";
        }
    }
}