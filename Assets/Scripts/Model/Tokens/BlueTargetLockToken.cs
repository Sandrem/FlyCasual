using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class BlueTargetLockToken : GenericTargetLockToken
    {

        public BlueTargetLockToken() {
            Name = "Blue Target Lock Token";
            Action = new ActionsList.TargetLockAction();
        }

    }

}
