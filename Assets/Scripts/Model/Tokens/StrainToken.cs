using Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class StrainToken : GenericToken
    {
        public StrainToken(GenericShip host) : base(host)
        {
            Name = ImageName = "Strain Token";
            Temporary = false;
            PriorityUI = 29;
            TokenColor = TokenColors.Red;
        }

    }

}
