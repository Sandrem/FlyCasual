using Upgrade;

namespace Ship
{
    namespace FirstEdition.StarViper
    {
        public class BlackSunVigo : StarViper
        {
            public BlackSunVigo() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "BlackSunVigo",
                    3,
                    27
                );

                ModelInfo.SkinName = "Black Sun Vigo";
            }
        }
    }
}
