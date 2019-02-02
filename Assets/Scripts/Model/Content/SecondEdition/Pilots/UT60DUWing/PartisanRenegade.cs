using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.UT60DUWing
    {
        public class PartisanRenegade : UT60DUWing
        {
            public PartisanRenegade() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Partisan Renegade",
                    1,
                    43,
                    extraUpgradeIcon: UpgradeType.Illicit,
                    seImageNumber: 61
                );

                ModelInfo.SkinName = "Partisan";
            }
        }
    }
}
