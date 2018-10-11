using Ship;
using Ship.LambdaShuttle;
using Upgrade;
using Abilities;
using RuleSets;
using System.Collections.Generic;
using ActionsList;
using System.Linq;
using System;
using Tokens;
using SubPhases;

namespace UpgradesList
{
    public class ST321 : GenericUpgrade, ISecondEditionUpgrade
    {
        public ST321() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "ST-321";
            Cost = 3;
            isUnique = true;

            UpgradeAbilities.Add(new ST321Ability());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 6;

            UpgradeAbilities.RemoveAll(a => a is ST321Ability);
            UpgradeAbilities.Add(new Abilities.SecondEdition.ST321Ability());

            SEImageNumber = 124;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is LambdaShuttle && ship.faction == Faction.Imperial;
        }
    }
}

namespace Abilities
{
    public class ST321Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.SetTargetLockRange(1, int.MaxValue);
        }

        public override void DeactivateAbility()
        {
            HostShip.SetTargetLockRange(1, 3);
        }
    }

    namespace SecondEdition
    {
        public class ST321Ability : GenericAbility
        {
            GenericShip coordinateTarget = null;

            public override void ActivateAbility()
            {
                HostShip.OnCoordinateTargetIsSelected += NoteDownCoordinateTarget;
                HostShip.OnActionIsPerformed += CheckSelectTargetForFreeTL;
            }

            public override void DeactivateAbility()
            {
                HostShip.OnCoordinateTargetIsSelected -= NoteDownCoordinateTarget;
                HostShip.OnActionIsPerformed -= CheckSelectTargetForFreeTL;
            }

            private void NoteDownCoordinateTarget(GenericShip ship)
            {
                coordinateTarget = ship;
            }

            private void CheckSelectTargetForFreeTL(GenericAction action)
            {
                if (action is CoordinateAction && coordinateTarget != null)
                {
                    var anyCandidate = Roster.AllShips.Values.Any(s => IsShipInRangeOfTarget(s));
                    if (anyCandidate)
                    {
                        RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, ST321Effect);
                    }
                }
            }

            private void ST321Effect(object sender, EventArgs e)
            {
                SelectTargetForAbility(
                      AssignLockToken,
                      IsShipInRangeOfTarget,
                      GetAiPriority,
                      HostShip.Owner.PlayerNo,
                      "Choose an enemy ship",
                      "Choose an enemy ship at range 0-3 of the ship you coordinated to acquire a lock on that enemy ship, ignoring range restrictions",
                      HostUpgrade
                  );
            }

            private void AssignLockToken()
            {
                Actions.AcquireTargetLock(HostShip, TargetShip, SelectShipSubPhase.FinishSelection, SelectShipSubPhase.FinishSelection, true);
            }

            private int HasTokenPriority(GenericShip ship)
            {
                if (ship.Tokens.HasToken(typeof(FocusToken))) return 100;
                if (ship.ActionBar.HasAction(typeof(EvadeAction)) || ship.Tokens.HasToken(typeof(EvadeToken))) return 50;
                if (ship.ActionBar.HasAction(typeof(TargetLockAction)) || ship.Tokens.HasToken(typeof(BlueTargetLockToken), '*')) return 50;
                return 0;
            }

            private int GetAiPriority(GenericShip ship)
            {
                int result = 0;

                result += HasTokenPriority(ship);
                result += ship.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.Cost);

                return result;
            }

            private bool IsShipInRangeOfTarget(GenericShip ship)
            {
                var match = BoardTools.Board.GetRangeOfShips(coordinateTarget, ship) <= 3;
                return match;
            }
        }
    }
}
