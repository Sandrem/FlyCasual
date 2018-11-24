using Abilities.SecondEdition;
using BoardTools;
using Ship;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.UT60DUWing
    {
        public class BodhiRook : UT60DUWing
        {
            public BodhiRook() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Bodhi Rook",
                    4,
                    49,
                    limited: 1,
                    abilityType: typeof(BodhiRookAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 54
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BodhiRookAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            RulesList.TargetLocksRule.OnCheckTargetLockIsAllowed += CanPerformTargetLock;
        }

        public override void DeactivateAbility()
        {
            RulesList.TargetLocksRule.OnCheckTargetLockIsAllowed -= CanPerformTargetLock;
        }

        private void CanPerformTargetLock(ref bool result, GenericShip ship, GenericShip defender)
        {
            if (ship.Owner.PlayerNo != HostShip.Owner.PlayerNo) return;

            foreach (GenericShip friendlyShip in HostShip.Owner.Ships.Values)
            {
                DistanceInfo distInfo = new DistanceInfo(friendlyShip, defender);
                if (distInfo.Range < 4)
                {
                    result = true;
                    return;
                }
            }
        }
    }
}
