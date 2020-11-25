using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.DroidTriFighter
{
    public class SeparatistInterceptor : DroidTriFighter
    {
        public SeparatistInterceptor()
        {
            PilotInfo = new PilotCardInfo(
                "Separatist Interceptor",
                3,
                37,
                extraUpgradeIcon: UpgradeType.Talent
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/e5/c8/e5c82543-69af-42ee-bb32-32b0c11d6845/swz81_separatist-interceptor_cutout.png";
        }
    }
}