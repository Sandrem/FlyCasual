using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class CloakToken : GenericToken
    {
        public CloakToken() {
            Name = "Cloak Token";
            Temporary = false;
            Tooltip = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/CloakAction.png";
        }

    }

}
