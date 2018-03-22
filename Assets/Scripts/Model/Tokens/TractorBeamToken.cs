using Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{
    public class TractorBeamToken : GenericToken
    {
        public Players.GenericPlayer Assigner;

        public TractorBeamToken(GenericShip host, Players.GenericPlayer assigner) : base(host)
        {
            Name = "Tractor Beam Token";
            Temporary = true;
            Tooltip = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/TractorBeamToken.png";
            Assigner = assigner;
        }
    }
}
