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
                    isLimited: true,
                    abilityType: typeof(ScourgeAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 84
                );

                ModelInfo.SkinName = "Inferno";
            }
        }
    }
}

