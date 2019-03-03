using Tokens;
using Ship;
using System.Collections.Generic;
using System.Linq;
using BoardTools;

namespace RulesList
{
    public class TargetLocksRule
    {
        public static event GenericShip.EventHandlerBool2Ships OnCheckTargetLockIsAllowed;
        public static event GenericShip.EventHandlerBool2Ships OnCheckTargetLockIsDisallowed;

        public void RegisterRemoveTargetLocksOnDestruction(GenericShip ship)
        {
            Triggers.RegisterTrigger(new Trigger
            {
                Name = "Remove tokens from destroyed ship",
                TriggerType = TriggerTypes.OnShipIsRemoved,
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

        public bool TargetLockIsAllowed(GenericShip ship, GenericShip target)
        {
            bool result = true;

            DistanceInfo distanceInfo = new DistanceInfo(ship, target);
            if (distanceInfo.Range > ship.TargetLockMaxRange || distanceInfo.Range < ship.TargetLockMinRange) result = false;

            if (result != true) if (OnCheckTargetLockIsAllowed != null) OnCheckTargetLockIsAllowed(ref result, ship, target);
            if (result == true) if (OnCheckTargetLockIsDisallowed != null) OnCheckTargetLockIsDisallowed(ref result, ship, target);

            return result;
        }
    } 
}