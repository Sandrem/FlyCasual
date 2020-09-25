using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.Eta2Actis
{
    public class Eta2ActisGeneric : Eta2Actis
    {
        public Eta2ActisGeneric()
        {
            PilotInfo = new PilotCardInfo(
                "Eta-2 Actis Generic",
                3,
                37,
                force: 1,
                extraUpgradeIcon: UpgradeType.ForcePower
            );
        }
    }
}