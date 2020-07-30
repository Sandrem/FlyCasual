using System;
using System.Collections.Generic;

namespace Ship.SecondEdition.VultureClassDroidFighter
{
    public class TradeFederationDrone : VultureClassDroidFighter
    {
        public TradeFederationDrone()
        {
            PilotInfo = new PilotCardInfo(
                "Trade Federation Drone",
                1,
                20
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/f0/05/f005d66c-754c-4bff-8ca2-45ea67e2d074/swz31_trade-federation-drone.png";
        }
    }
}