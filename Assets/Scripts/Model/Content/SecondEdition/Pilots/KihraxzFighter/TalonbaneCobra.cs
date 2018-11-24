using Ship;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.KihraxzFighter
    {
        public class TalonbaneCobra : KihraxzFighter
        {
            public TalonbaneCobra() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Talonbane Cobra",
                    5,
                    50,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.TalonbaneCobraAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 191
                );
            }
        }
    }
}

