using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;

namespace Arcs
{
    public class Arc180 : GenericArc
    {
        public Arc180(GenericShipBase shipBase) : base(shipBase)
        {
            ArcType = ArcTypes.Special;
            Facing = ArcFacing.Forward180;
            MinAngle = -90f;
            MaxAngle = 90f;
            ShotPermissions = new ArcShotPermissions()
            {
                CanShootPrimaryWeapon = true,
            };
        }
    }
}
