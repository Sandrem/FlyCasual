﻿using Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class BlueTargetLockToken : GenericTargetLockToken
    {

        public BlueTargetLockToken(GenericShip host) : base(host)
        {
            Name = "Blue Target Lock Token";
            Action = new ActionsList.TargetLockAction() { Host = host };
        }

    }

}
