using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class JinataSecurityOfficer : BTLA4YWing
        {
            public JinataSecurityOfficer() : base()
            {
                RequiredMods = new List<Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

                PilotInfo = new PilotCardInfo
                (
                    "Jinata Security Officer",
                    2,
                    32,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Tech,
                        UpgradeType.Illicit
                    },
                    factionOverride: Faction.Scum
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/e6/7f/e67f3145-67ad-4175-8a48-b92d87e58c28/swz85_pilot_jinatasecurityofficer.png";
            }
        }
    }
}
