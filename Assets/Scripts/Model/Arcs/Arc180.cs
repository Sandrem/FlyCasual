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
            ArcsList = new List<ArcInfo>
            {
                primaryArc,
                new ArcInfo()
                {
                    ShipBase = Host.ShipBase,
                    MinAngle = -90f,
                    MaxAngle = 90f,
                    Facing = ArcFacing.Forward180
                }
            };
        }
    }
}
