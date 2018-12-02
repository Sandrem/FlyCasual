using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Upgrade
{
    interface IVariableCost
    {
        void UpdateCost(GenericShip ship);
    }
}
