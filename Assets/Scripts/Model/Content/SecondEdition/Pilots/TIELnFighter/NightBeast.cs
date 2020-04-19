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
                    25,
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
        protected override string AbilityDescription => "After you fully execute a blue maneuver, you may perform a Focus action";

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
