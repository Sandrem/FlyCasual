using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class WeaponsDisabledToken : GenericToken
    {
        public WeaponsDisabledToken() {
            Name = "Weapons Disabled Token";
            Temporary = true;
            Tooltip = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/SlamAction.png";
        }

    }

}
