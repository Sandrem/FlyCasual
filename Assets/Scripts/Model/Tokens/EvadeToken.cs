using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class EvadeToken : GenericToken
    {
        public EvadeToken() {
            Name = "Evade Token";
            Action = new Actions.EvadeAction();
        }

    }

}
