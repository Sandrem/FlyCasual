using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;

namespace Arcs
{
    public class ArcBullseye : GenericArc
    {
        public ArcBullseye(GenericShip host) : base(host)
        {
            ArcsList = new List<ArcInfo>
            {
                primaryArc,
                new ArcInfo()
                {
                    ShipBase = Host.ShipBase,
                    MinAngle = 0f,
                    MaxAngle = 0f,
                    Facing = ArcFacing.Bullseye
                }
            };
        }
    }
}
