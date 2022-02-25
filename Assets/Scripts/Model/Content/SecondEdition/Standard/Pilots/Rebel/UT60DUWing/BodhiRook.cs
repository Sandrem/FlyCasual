using Abilities.SecondEdition;
using Ship;
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
                PilotInfo = new PilotCardInfo25
                (
                    "Bodhi Rook",
                    "Imperial Defector",
                    Faction.Rebel,
                    4,
                    5,
                    10,
                    isLimited: true,
                    abilityType: typeof(BodhiRookAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Sensor,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Modification
                    },
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

        private void CanPerformTargetLock(ref bool result, GenericShip ship, ITargetLockable defender)
        {
            if (ship.Owner.PlayerNo != HostShip.Owner.PlayerNo) return;

            foreach (GenericShip friendlyShip in HostShip.Owner.Ships.Values)
            {
                if (defender.GetRangeToShip(friendlyShip) < 4)
                {
                    result = true;
                    return;
                }
            }
        }
    }
}
