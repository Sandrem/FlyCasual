namespace Ship
{
    namespace FirstEdition.ProtectorateStarfighter
    {
        public class ZealousRecruit : ProtectorateStarfighter
        {
            public ZealousRecruit() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Zealous Recruit",
                    1,
                    20
                );
            }
        }
    }
}