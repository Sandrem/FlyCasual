using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEFoFighter
    {
        public class ZetaSquadronPilot : TIEFoFighter
        {
            public ZetaSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Zeta Squadron Pilot",
                    2,
                    29 //,
                    //seImageNumber: 120
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/c0/af/c0afde49-7f44-4c59-8051-cc4140a04be0/swz26_a1_zeta-pilot.png";
            }
        }
    }
}
