using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.Belbullab22Starfighter
{
    public class FeethanOttrawAutopilot : Belbullab22Starfighter
    {
        public FeethanOttrawAutopilot()
        {
            PilotInfo = new PilotCardInfo(
                "Feethan Ottraw Autopilot",
                1,
                37
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/6e/76/6e7626dc-3b81-4290-aece-ddf5d86e7667/swz29_autopilot.png";

            ShipInfo.ActionIcons.SwitchToDroidActions();
        }
    }
}