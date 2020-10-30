using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.DroidTriFighter
{
    public class ColicoidInterceptor : DroidTriFighter
    {
        public ColicoidInterceptor()
        {
            PilotInfo = new PilotCardInfo(
                "Colicoid Interceptor",
                1,
                31
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/74/88/7488fd46-8f27-4ca9-b573-db8d6f7d749e/swz81_colicoid-interceptor_cutout.png";
        }
    }
}