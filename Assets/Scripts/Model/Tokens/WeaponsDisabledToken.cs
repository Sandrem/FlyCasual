﻿using Editions;
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
            Name = ImageName = (Edition.Current is SecondEdition) ? "Disarm Token" : "Weapons Disabled Token";
            Temporary = true;
            PriorityUI = 45;
            TokenColor = TokenColors.Orange;
            TokenShape = TokenShapes.Cirular;
            Tooltip = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/SlamAction.png";
        }

    }

}
