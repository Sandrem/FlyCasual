using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.DroidTriFighter
{
    public class ColicoidInterceptor : DroidTriFighter
    {
        public ColicoidInterceptor()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Colicoid Interceptor",
                "",
                Faction.Separatists,
                1,
                4,
                4,
                extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent
                    },
                tags: new List<Tags>
                {
                    Tags.Droid
                }
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/74/88/7488fd46-8f27-4ca9-b573-db8d6f7d749e/swz81_colicoid-interceptor_cutout.png";
        }
    }
}