using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEFoFighter
    {
        public class OmegaSquadronAce : TIEFoFighter
        {
            public OmegaSquadronAce() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Omega Squadron Ace",
                    3,
                    31,
                    extraUpgradeIcon: UpgradeType.Talent //,
                    //seImageNumber: 120
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/5f/de/5fde2469-451e-46eb-ad4e-936ff1d86935/swz26_a1_omega-ace.png";
            }
        }
    }
}
