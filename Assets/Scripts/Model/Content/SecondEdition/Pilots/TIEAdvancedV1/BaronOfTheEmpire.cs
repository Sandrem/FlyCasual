using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEAdvancedV1
    {
        public class BaronOfTheEmpire : TIEAdvancedV1
        {
            public BaronOfTheEmpire() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Baron of the Empire",
                    3,
                    34,
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 101
                );
            }
        }
    }
}