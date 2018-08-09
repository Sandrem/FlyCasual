using Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class StressToken : GenericToken
    {
        public StressToken(GenericShip host) : base(host)
        {
            Name = "Stress Token";
            Temporary = false;
            PriorityUI = 30;
            TokenColor = TokenColors.Red;
        }

    }

}
