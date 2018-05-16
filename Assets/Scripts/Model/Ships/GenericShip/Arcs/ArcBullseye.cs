using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;

namespace Arcs
{
    public class ArcBullseye : GenericArc
    {
        public ArcBullseye(GenericShipBase shipBase) : base(shipBase)
        {
            ArcType = ArcTypes.Bullseye;
            Facing = ArcFacing.Bullseye;
            MinAngle = 0f;
            MaxAngle = 0f;
            ShotPermissions = new ArcShotPermissions();
        }
    }
}
