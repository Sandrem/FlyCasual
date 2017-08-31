using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;

namespace Arcs
{
    public class Arc180 : GenericArc
    {
        public Arc180(GenericShip host) : base(host)
        {
            attackAngles = new List<ArcInfo> { new ArcInfo(-90f, 90f) };
        }
    }
}
