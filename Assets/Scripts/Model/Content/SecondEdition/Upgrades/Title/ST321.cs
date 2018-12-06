using ActionsList;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class ST321 : GenericUpgrade
    {
        public ST321() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "ST-321",
                UpgradeType.Title,
                cost: 6,
                isLimited: true,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.LambdaClassT4AShuttle.LambdaClassT4AShuttle)),
                abilityType: typeof(Abilities.SecondEdition.ST321Ability),
                seImageNumber: 124
            );
        }        
    }
}

namespace Abilities.SecondEdition
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
            ActionsHolder.AcquireTargetLock(HostShip, TargetShip, SelectShipSubPhase.FinishSelection, SelectShipSubPhase.FinishSelection, true);
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
            result += ship.PilotInfo.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.UpgradeInfo.Cost);

            return result;
        }

        private bool IsShipInRangeOfTarget(GenericShip ship)
        {
            var match = BoardTools.Board.GetRangeOfShips(coordinateTarget, ship) <= 3;
            return match;
        }
    }
}