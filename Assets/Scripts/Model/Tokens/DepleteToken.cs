using Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class DepleteToken : GenericToken
    {
        public DepleteToken(GenericShip host) : base(host)
        {
            Name = ImageName = "Deplete Token";
            Temporary = false;
            PriorityUI = 28;
            TokenColor = TokenColors.Red;
        }

    }

}
