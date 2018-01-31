using Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class IonToken : GenericToken
    {
        public IonToken(GenericShip host) : base(host)
        {
            Name = "Ion Token";
            Temporary = false;
            Tooltip = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/IonToken.png";
        }

    }

}
