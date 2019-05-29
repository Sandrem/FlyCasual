using Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class RedTargetLockToken : GenericTargetLockToken
    {
        public new ITargetLockable Host { get; private set; }

        public RedTargetLockToken(ITargetLockable host) : base(null)
        {
            Name = "Red Target Lock Token";
            TokenColor = TokenColors.Red;
            PriorityUI = 20;

            Host = host;
        }
    }

}
