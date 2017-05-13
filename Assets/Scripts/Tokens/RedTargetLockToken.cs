using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class RedTargetLockToken : GenericToken
    {
        public RedTargetLockToken() {
            Name = "Red Target Lock Token";
            Temporary = false;
            Action = new Actions.TargetLockAction();
        }
    }

}
