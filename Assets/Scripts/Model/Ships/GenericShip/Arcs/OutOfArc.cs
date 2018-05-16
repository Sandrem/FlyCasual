using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;

namespace Arcs
{
    public class OutOfArc : GenericArc
    {

        public OutOfArc(GenericShipBase shipBase) : base(shipBase)
        {
            ArcType = ArcTypes.None;
            Facing = ArcFacing.None;
            MinAngle = -360f;
            MaxAngle = 360f;
            ShotPermissions = new ArcShotPermissions(); //Cannot shoot out of arc by default
        }
    }
}
