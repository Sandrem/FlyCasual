using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;

namespace Arcs
{
    public class Arc360 : GenericArc
    {

        public Arc360(GenericShip host) : base(host)
        {
            CanShootOutsideArc = true;
        }

        public override bool InAttackAngle(float angle)
        {
            return true;
        }
    }
}
