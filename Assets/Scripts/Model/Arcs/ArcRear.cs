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
            ArcsList = new List<ArcInfo>
            {
                primaryArc,
                new ArcInfo()
                {
                    ShipBase = Host.ShipBase,
                    MinAngle = -140f,
                    MaxAngle = 140f,
                    Facing = ArcFacing.Rear
                }
            };
        }
    }
}
