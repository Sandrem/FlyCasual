using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.V19TorrentStarfighter
{
    public class BlueSquadronProtector : V19TorrentStarfighter
    {
        public BlueSquadronProtector()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Blue Squadron Protector",
                "",
                Faction.Republic,
                3,
                4,
                4,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent
                },
                tags: new List<Tags>
                {
                    Tags.Clone
                }
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/51/18/5118d916-b09f-47a1-a4dd-0df667267d1b/swz32_blue-sqd-protector.png";
        }
    }
}