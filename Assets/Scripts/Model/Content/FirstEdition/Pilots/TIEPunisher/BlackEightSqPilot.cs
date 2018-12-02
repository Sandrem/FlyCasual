namespace Ship
{
    namespace FirstEdition.TIEPunisher
    {
        public class BlackEightSqPilot : TIEPunisher
        {
            public BlackEightSqPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Black Eight Sq. Pilot",
                    4,
                    23
                );

                // ImageUrl = ImageUrls.GetImageUrl(this, "black-eight-squadron-pilot.png");
            }
        }
    }
}
