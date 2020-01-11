﻿namespace Ship
{
    namespace SecondEdition.FangFighter
    {
        public class ZealousRecruit : FangFighter
        {
            public ZealousRecruit() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Zealous Recruit",
                    1,
                    41,
                    seImageNumber: 160
                );
            }
        }
    }
}