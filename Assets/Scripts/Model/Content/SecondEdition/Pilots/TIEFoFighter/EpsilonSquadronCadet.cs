using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEFoFighter
    {
        public class EpsilonSquadronCadet : TIEFoFighter
        {
            public EpsilonSquadronCadet() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Epsilon Squadron Cadet",
                    1,
                    28 //,
                    //seImageNumber: 120
                );

                ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/1/18/Swz26_a1_epsilon-pilot.png";
            }
        }
    }
}
