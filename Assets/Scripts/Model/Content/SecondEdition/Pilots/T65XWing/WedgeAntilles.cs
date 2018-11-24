using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class WedgeAntilles : T65XWing
        {
            public WedgeAntilles() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Wedge Antilles",
                    6,
                    52,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.WedgeAntillesAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 1
                );
            }
        }
    }
}
