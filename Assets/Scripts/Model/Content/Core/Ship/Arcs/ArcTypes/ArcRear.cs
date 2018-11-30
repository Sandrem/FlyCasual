﻿using System;
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
            ArcType = ArcType.RearAux;
            Facing = ArcFacing.Rear;

            Limits = new Dictionary<Vector3, float>()
            {
                { new Vector3(-shipBase.HALF_OF_FIRINGARC_SIZE, 0, -shipBase.SHIPSTAND_SIZE), -140f },
                { new Vector3( shipBase.HALF_OF_FIRINGARC_SIZE, 0, -shipBase.SHIPSTAND_SIZE),  140f }
            };

            Edges = new List<Vector3>()
            {
                new Vector3(-shipBase.HALF_OF_FIRINGARC_SIZE, 0, -shipBase.SHIPSTAND_SIZE),
                new Vector3( shipBase.HALF_OF_FIRINGARC_SIZE, 0, -shipBase.SHIPSTAND_SIZE),
            };

            ShotPermissions = new ArcShotPermissions()
            {
                CanShootPrimaryWeapon = true,
            };
        }
    }
}
