using Abilities.SecondEdition;
using Ship;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class NightBeast : TIELnFighter
        {
            public NightBeast() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Night Beast",
                    2,
                    26,
                    isLimited: true,
                    abilityType: typeof(NightBeastAbility),
                    seImageNumber: 88
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class NightBeastAbility : Abilities.FirstEdition.NightBeastAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += NightBeastPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= NightBeastPilotAbility;
        }
    }
}
