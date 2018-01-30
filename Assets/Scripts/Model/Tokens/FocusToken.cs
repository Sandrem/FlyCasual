using Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class FocusToken : GenericToken
    {
        public FocusToken(GenericShip host) : base(host)
        {
            Name = "Focus Token";
            Action = new ActionsList.FocusAction() { Host = host};
        }
    }

}
