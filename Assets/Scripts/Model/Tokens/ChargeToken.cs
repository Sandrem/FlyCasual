using Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class ChargeToken : GenericToken
    {
        public ChargeToken(GenericShip host) : base(host)
        {
            Name = "Charge Token";
            PriorityUI = 90;
            Temporary = false;
        }
    }

}
