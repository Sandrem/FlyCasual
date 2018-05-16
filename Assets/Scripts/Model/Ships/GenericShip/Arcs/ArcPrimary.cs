using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;

namespace Arcs
{
    public class ArcPrimary : GenericArc
    {
        public ArcPrimary(GenericShipBase shipBase) : base(shipBase)
        {
            ArcType = ArcTypes.Primary;
            Facing = ArcFacing.Front;
            MinAngle = -40f;
            MaxAngle = 40f;
            ShotPermissions = new ArcShotPermissions()
            {
                CanShootPrimaryWeapon = true,
                CanShootCannon = true,
                CanShootMissiles = true,
                CanShootTorpedoes = true,
                CanShootTurret = true
            };
        }
    }
}
