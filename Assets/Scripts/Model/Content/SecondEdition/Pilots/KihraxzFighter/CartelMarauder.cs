using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.KihraxzFighter
    {
        public class CartelMarauder : KihraxzFighter
        {
            public CartelMarauder() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Cartel Marauder",
                    2,
                    40,
                    seImageNumber: 196
                );
            }
        }
    }
}
