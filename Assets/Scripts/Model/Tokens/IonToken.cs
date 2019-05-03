using Editions;
using Ship;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tokens
{

    public class IonToken : GenericToken
    {
        public IonToken(GenericShip host) : base(host)
        {
            Name = "Ion Token";
            Temporary = false;
            TokenColor = TokenColors.Red;
            PriorityUI = 35;
            Tooltip = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/IonToken.png";
        }

        public override void WhenAssigned()
        {
            if (IsIonized())
            {
                Messages.ShowInfo("This ship is ionized!");
                Host.ToggleIonized(true);
            }
        }

        public bool IsIonized()
        {
            int ionTokensCount = Host.Tokens.GetAllTokens().Count(n => n is IonToken);
            return (ionTokensCount >= Edition.Current.NegativeTokensToAffectShip[Host.ShipInfo.BaseSize]);
        }

        public override void WhenRemoved()
        {
            if (!IsIonized())
            {
                Host.ToggleIonized(false);
            }
        }

    }

}
