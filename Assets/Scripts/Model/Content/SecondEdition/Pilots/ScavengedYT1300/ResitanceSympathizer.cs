using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ScavengedYT1300
    {
        public class ResitanceSympathizer : ScavengedYT1300
        {
            public ResitanceSympathizer() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Resitance Sympathizer",
                    2,
                    68 //,
                    //seImageNumber: 69
                );

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/28411b84c1b15f0bfa9928f2206e44f5.png";
            }
        }
    }
}