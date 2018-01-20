﻿using Ship;

namespace Tokens
{

    public class ReinforceAftToken : GenericToken
    {
        public ReinforceAftToken(GenericShip host): base(host)
        {
            Name = "Reinforce Aft Token";
            Action = new ActionsList.ReinforceAftAction() { Host = this.Host };
        }
    }

}
