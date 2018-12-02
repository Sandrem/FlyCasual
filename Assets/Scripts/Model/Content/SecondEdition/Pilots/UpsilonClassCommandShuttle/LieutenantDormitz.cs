using Upgrade;

namespace Ship
{
    namespace SecondEdition.UpsilonClassCommandShuttle
    {
        public class LieutenantDormitz : UpsilonClassCommandShuttle
        {
            public LieutenantDormitz() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Lieutenant Dormitz",
                    2,
                    60,
                    isLimited: true //,
                    //seImageNumber: 120
                );

                ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/e/e1/Swz18_lt-dormitz_a2.png";
            }
        }
    }
}
