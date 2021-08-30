using Ship;
using Tokens;

namespace RulesList
{
    public class RemotesRule
    {
        public void AllowOnlyLocks(GenericShip ship, GenericToken token)
        {
            if (!(token is RedTargetLockToken))
            {
                Messages.ShowInfo("Remotes cannot be assigned tokens except for locks");
                ship.Tokens.TokenToAssign = null;
            }
        }
    }
}
