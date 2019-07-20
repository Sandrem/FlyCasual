using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokens;
using UnityEngine;
using Ship;

namespace Remote
{
    public class RemoteTokensHolder : TokensManager
    {
        public RemoteTokensHolder(GenericShip host) : base(host)
        {

        }
    }
}
