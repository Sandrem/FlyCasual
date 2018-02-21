using Tokens;
using Ship;
using System.Collections.Generic;
using System.Linq;

namespace RulesList
{
    public class TargetLocksRule
    {
        public static event GenericShip.EventHandler2Ships OnCheckTargetLockIsAllowed;

        public void RegisterRemoveTargetLocksOnDestruction(GenericShip ship, bool isFled)
        {
            Triggers.RegisterTrigger(new Trigger
            {
                Name = "Remove tokens from destroyed ship",
                TriggerType = TriggerTypes.OnShipIsDestroyed,
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

            Actions.RemoveTokens(tokensToRemove, Triggers.FinishTrigger);
        }

        public bool TargetLockIsAllowed(GenericShip attacker, GenericShip target)
        {
            bool result = true;

            if (OnCheckTargetLockIsAllowed != null) OnCheckTargetLockIsAllowed(ref result, attacker, target);
            return result;
        }
    } 
}