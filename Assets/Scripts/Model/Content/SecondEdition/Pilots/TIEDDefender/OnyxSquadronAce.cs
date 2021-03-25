using Upgrade;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.TIEDDefender
    {
        public class OnyxSquadronAce : TIEDDefender
        {
            public OnyxSquadronAce() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Onyx Squadron Ace",
                    4,
                    73,
                    extraUpgradeIcons: new List<UpgradeType>(){ UpgradeType.Talent, UpgradeType.Sensor },
                    seImageNumber: 125
                );
            }
        }
    }
}
