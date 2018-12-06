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

                ImageUrl = "http://www.infinitearenas.com/xw2browse/images/resistance/resistance-sympathizer.jpg";
            }
        }
    }
}