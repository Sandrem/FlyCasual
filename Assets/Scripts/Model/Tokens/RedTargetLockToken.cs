using Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class RedTargetLockToken : GenericTargetLockToken
    {
        public RedTargetLockToken(GenericShip host) : base(host)
        {
            Name = "Red Target Lock Token";
            TokenColor = TokenColors.Red;
            PriorityUI = 20;
        }
    }

}
