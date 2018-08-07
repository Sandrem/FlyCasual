using RuleSets;
using Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class JamToken : GenericToken
    {
        public JamToken(GenericShip host) : base(host)
        {
            Name = "Jam Token";
            Temporary = RuleSet.Instance is SecondEdition;
            TokenColor = TokenColors.Yellow;
            PriorityUI = 40;
            Tooltip = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/ReloadActionAndJamTokens.png";
        }
    }

}
