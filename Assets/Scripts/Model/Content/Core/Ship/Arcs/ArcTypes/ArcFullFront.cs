using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;

namespace Arcs
{
    public class ArcFullFront : GenericArc
    {
        public ArcFullFront(GenericShipBase shipBase) : base(shipBase)
        {
            ArcType = ArcType.SpecialGhost;
            Facing = ArcFacing.FullFront;

            Limits = new Dictionary<Vector3, float>()
            {
                { new Vector3(-shipBase.HALF_OF_SHIPSTAND_SIZE, 0, -shipBase.HALF_OF_SHIPSTAND_SIZE), -90f },
                { new Vector3(shipBase.HALF_OF_SHIPSTAND_SIZE,  0, -shipBase.HALF_OF_SHIPSTAND_SIZE),  90f }
            };

            Edges = new List<Vector3>()
            {
                new Vector3(-shipBase.HALF_OF_SHIPSTAND_SIZE, 0, 0),
                new Vector3( shipBase.HALF_OF_SHIPSTAND_SIZE, 0, 0),
                new Vector3(-shipBase.HALF_OF_SHIPSTAND_SIZE, 0, -shipBase.HALF_OF_SHIPSTAND_SIZE),
                new Vector3( shipBase.HALF_OF_SHIPSTAND_SIZE, 0, -shipBase.HALF_OF_SHIPSTAND_SIZE)
            };

            ShotPermissions = new ArcShotPermissions()
            {
                CanShootPrimaryWeapon = true,
            };
        }
    }
}
