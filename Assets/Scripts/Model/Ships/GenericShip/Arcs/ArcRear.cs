using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;

namespace Arcs
{
    public class ArcRear : GenericArc
    {
        public ArcRear(GenericShipBase shipBase) : base(shipBase)
        {
            ArcType = ArcTypes.RearAux;
            Facing = ArcFacing.Rear;
            MinAngle = -140f;
            MaxAngle = 140f;
            ShotPermissions = new ArcShotPermissions()
            {
                CanShootPrimaryWeapon = true,
            };
        }
    }
}
