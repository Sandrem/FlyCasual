using Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class CalculateToken : GenericToken
    {
        public CalculateToken(GenericShip host) : base(host)
        {
            Name = "Calculate Token";
            TokenColor = TokenColors.Green;
            Action = new ActionsList.CalculateAction() { Host = host};
        }
    }

}
