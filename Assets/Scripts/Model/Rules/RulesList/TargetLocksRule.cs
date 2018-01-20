using Tokens;
using Ship;

namespace RulesList
{
    public class TargetLocksRule
    {
        public static event GenericShip.EventHandler2Ships OnCheckTargetLockIsAllowed;

        public void RemoveTargetLocksOnDestruction(GenericShip ship)
        {
            while (true)
            {
                BlueTargetLockToken token = ship.GetToken(typeof(BlueTargetLockToken), '*') as BlueTargetLockToken;
                if (token != null)
                {
                    ship.RemoveToken(token.GetType(), token.Letter);
                }
                else
                {
                    break;
                }
            }

            while (true)
            {
                RedTargetLockToken token = ship.GetToken(typeof(RedTargetLockToken), '*') as RedTargetLockToken;
                if (token != null)
                {
                    ship.RemoveToken(token.GetType(), token.Letter);
                }
                else
                {
                    break;
                }
            }

        }

        public bool TargetLockIsAllowed(GenericShip attacker, GenericShip target)
        {
            bool result = true;

            if (OnCheckTargetLockIsAllowed != null) OnCheckTargetLockIsAllowed(ref result, attacker, target);
            return result;
        }
    } 
}