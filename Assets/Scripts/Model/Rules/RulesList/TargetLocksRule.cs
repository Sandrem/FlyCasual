using Tokens;
using Ship;
using System.Collections.Generic;
using System.Linq;
using BoardTools;

namespace RulesList
{
    public class TargetLocksRule
    {
        public static event GenericShip.EventHandlerBoolShipLock OnCheckTargetLockIsAllowed;
        public static event GenericShip.EventHandlerBoolShipLock OnCheckTargetLockIsDisallowed;

        public void RegisterRemoveTargetLocksOnDestruction(GenericShip ship)
        {
            Triggers.RegisterTrigger(new Trigger
            {
                Name = "Remove tokens from destroyed ship",
                TriggerType = TriggerTypes.OnShipIsReadyToBeRemoved,
                TriggerOwner = ship.Owner.PlayerNo,
                EventHandler = DoRemoveTargetLocksOnDestruction,
                Sender = ship
            });
        }

        public void DoRemoveTargetLocksOnDestruction(object sender, System.EventArgs e)
        {
            GenericShip ship = sender as GenericShip;

            List<GenericToken> tokensToRemove = new List<GenericToken>();
            tokensToRemove.AddRange(ship.Tokens.GetAllTokens().Where(n => n is BlueTargetLockToken || n is RedTargetLockToken));

            ActionsHolder.RemoveTokens(tokensToRemove, Triggers.FinishTrigger);
        }

        public bool TargetLockIsAllowed(GenericShip ship, ITargetLockable target)
        {
            bool result = true;

            int rangeBetween = target.GetRangeToShip(ship);
            if (rangeBetween > ship.TargetLockMaxRange || rangeBetween < ship.TargetLockMinRange) result = false;

            if (result != true) OnCheckTargetLockIsAllowed?.Invoke(ref result, ship, target);
            if (result == true) OnCheckTargetLockIsDisallowed?.Invoke(ref result, ship, target);

            return result;
        }
    } 
}