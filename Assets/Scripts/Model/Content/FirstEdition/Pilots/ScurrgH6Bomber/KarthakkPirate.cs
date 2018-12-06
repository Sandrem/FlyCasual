namespace Ship
{
    namespace FirstEdition.ScurrgH6Bomber
    {
        public class KarthakkPirate : ScurrgH6Bomber
        {
            public KarthakkPirate() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Karthakk Pirate",
                    1,
                    24
                );

                ModelInfo.SkinName = "Karthakk Pirate";
            }
        }
    }
}
