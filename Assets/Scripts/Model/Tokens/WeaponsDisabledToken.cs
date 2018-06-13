using Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class WeaponsDisabledToken : GenericToken
    {
        public WeaponsDisabledToken(GenericShip host) : base(host)
        {
            Name = "Weapons Disabled Token";
            Temporary = true;
            TokenColor = TokenColors.Yellow;
            Tooltip = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/SlamAction.png";
        }

    }

}
