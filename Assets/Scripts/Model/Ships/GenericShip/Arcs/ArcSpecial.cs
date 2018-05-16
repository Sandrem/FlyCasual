using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;

namespace Arcs
{
    public class ArcSpecial : GenericArc
    {
        public ArcSpecial(GenericShipBase shipBase) : base(shipBase)
        {
            ArcType = ArcTypes.Special;
            Facing = ArcFacing.Rear;
            MinAngle = -140f;
            MaxAngle = 140f;
            ShotPermissions = new ArcShotPermissions()
            {
                CanShootTorpedoes = true,
            };
        }
    }
}
