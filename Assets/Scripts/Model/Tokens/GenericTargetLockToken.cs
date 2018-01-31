﻿using Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class GenericTargetLockToken : GenericToken
    {
        public char Letter;
        public Ship.GenericShip OtherTokenOwner;

        public GenericTargetLockToken(GenericShip host) : base(host)
        {
            Temporary = false;
        }
    }

}
