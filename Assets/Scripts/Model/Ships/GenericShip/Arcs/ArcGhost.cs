using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;

namespace Arcs
{
    public class ArcGhost : GenericArc
    {
        public ArcGhost(GenericShip host) : base(host)
        {
            ArcsList = new List<ArcInfo>
            {
                primaryArc,
                new ArcInfo()
                {
                    ShipBase = Host.ShipBase,
                    MinAngle = -140f,
                    MaxAngle = 140f,
                    Facing = ArcFacing.Rear,
                    ShotPermissions = new ArcShotPermissions()
                    {
                        CanShootPrimaryWeapon = false, CanShootTorpedoes = true
                    }
                }
            };
        }
    }
}
