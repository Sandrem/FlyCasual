using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class JamToken : GenericToken
    {
        public JamToken() {
            Name = "Jam Token";
            Temporary = false;
            Tooltip = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/ReloadActionAndJamTokens.png";
        }

    }

}
