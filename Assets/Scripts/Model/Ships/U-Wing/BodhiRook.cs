using BoardTools;
using RuleSets;
using Ship;
using Upgrade;

namespace Ship
{
    namespace UWing
    {
        public class BodhiRook : UWing, ISecondEditionPilot
        {
            public BodhiRook() : base()
            {
                PilotName = "Bodhi Rook";
                PilotSkill = 4;
                Cost = 49;

                IsUnique = true;

                PrintedUpgradeIcons.Add(UpgradeType.Elite);

                PilotRuleType = typeof(SecondEdition);

                PilotAbilities.Add(new Abilities.SecondEdition.BodhiRookAbilitySE());

                SEImageNumber = 54;
            }

            public void AdaptPilotToSecondEdition()
            {
                // not needed
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BodhiRookAbilitySE : GenericAbility
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
