using Actions;
using ActionsList;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using UnityEngine;

namespace RulesList
{
    public class RemotesRule
    {
        public void AllowOnlyLocks(GenericShip ship, Type type)
        {
            if (type != typeof(RedTargetLockToken))
            {
                Messages.ShowInfo("Remotes cannot be assigned tokens except for locks");
                ship.Tokens.TokenToAssign = null;
            }
        }
    }
}
