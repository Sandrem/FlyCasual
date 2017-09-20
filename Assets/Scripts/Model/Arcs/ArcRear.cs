using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;

namespace Arcs
{
    public class ArcRear : GenericArc
    {
        public ArcRear(GenericShip host) : base(host)
        {
            attackAngles = new List<ArcInfo> { new ArcInfo(-40f, 40f), new ArcInfo(-140f, 140f, true) };
        }
    }
}
