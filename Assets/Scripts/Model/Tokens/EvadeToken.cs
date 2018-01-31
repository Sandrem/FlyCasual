﻿using Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class EvadeToken : GenericToken
    {
        public EvadeToken(GenericShip host) : base(host)
        {
            Name = "Evade Token";
            Action = new ActionsList.EvadeAction() { Host = host };
        }

    }

}
