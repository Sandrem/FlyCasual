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
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.TalonbaneCobraAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 191
                );
            }
        }
    }
}

