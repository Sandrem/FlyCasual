using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.V19TorrentStarfighter
{
    public class GoldSquadronTrooper : V19TorrentStarfighter
    {
        public GoldSquadronTrooper()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Gold Squadron Trooper",
                "",
                Faction.Republic,
                2,
                4,
                6,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Missile
                },
                tags: new List<Tags>
                {
                    Tags.Clone
                }
            );
            
            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/ab/a7/aba7c941-3472-453f-bfd8-032f5f854e0b/swz32_gold-sqd-trooper.png";
        }
    }
}