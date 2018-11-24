using System.Collections;
using System.Collections.Generic;
using Abilities.FirstEdition;
using BoardTools;
using Arcs;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class GideonHask : TIELnFighter
        {
            public GideonHask() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Gideon Hask",
                    4,
                    30,
                    limited: 1,
                    abilityType: typeof(ScourgeAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 84
                );
            }
        }
    }
}

