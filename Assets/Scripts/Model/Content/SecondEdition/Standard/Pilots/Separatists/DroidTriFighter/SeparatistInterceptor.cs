using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.DroidTriFighter
{
    public class SeparatistInterceptor : DroidTriFighter
    {
        public SeparatistInterceptor()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Separatist Interceptor",
                "",
                Faction.Separatists,
                3,
                4,
                3,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Missile
                },
                tags: new List<Tags>
                {
                    Tags.Droid
                }
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/e5/c8/e5c82543-69af-42ee-bb32-32b0c11d6845/swz81_separatist-interceptor_cutout.png";
        }
    }
}