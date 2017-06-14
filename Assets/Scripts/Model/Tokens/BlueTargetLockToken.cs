using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class BlueTargetLockToken : GenericTargetLockToken
    {
        public Ship.GenericShip LockedShip;

        public BlueTargetLockToken() {
            Name = "Blue Target Lock Token";
            Action = new ActionsList.TargetLockAction();
        }

    }

}
