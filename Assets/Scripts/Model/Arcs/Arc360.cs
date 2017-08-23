using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;

namespace Arcs
{
    public class Arc360 : GenericArc
    {
        public override bool InAttackAngle(GenericShip targetShip)
        {
            return true;
        }
    }
}
