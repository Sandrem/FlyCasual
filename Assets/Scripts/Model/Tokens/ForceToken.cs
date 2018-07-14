using Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class ForceToken : GenericToken
    {
        public ForceToken(GenericShip host) : base(host)
        {
            Name = "Force Token";
            Temporary = false;
        }
    }

}
