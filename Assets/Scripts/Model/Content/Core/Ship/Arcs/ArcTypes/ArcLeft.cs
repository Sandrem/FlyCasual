using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;

namespace Arcs
{
    public class ArcLeft : GenericArc
    {
        public ArcLeft(GenericShipBase shipBase) : base(shipBase)
        {
            ArcType = ArcType.Left;
            Facing = ArcFacing.Left;

            Limits = new Dictionary<Vector3, float>()
            {
                { new Vector3(-shipBase.HALF_OF_FIRINGARC_SIZE, 0, -shipBase.SHIPSTAND_SIZE), -140f },
                { new Vector3(-shipBase.HALF_OF_FIRINGARC_SIZE, 0, 0),  -40f }
            };

            Edges = new List<Vector3>()
            {
                new Vector3(-shipBase.HALF_OF_FIRINGARC_SIZE, 0, 0),
                new Vector3(-shipBase.HALF_OF_SHIPSTAND_SIZE, 0, 0),
                new Vector3(-shipBase.HALF_OF_SHIPSTAND_SIZE, 0, -shipBase.SHIPSTAND_SIZE),
                new Vector3(-shipBase.HALF_OF_FIRINGARC_SIZE, 0, -shipBase.SHIPSTAND_SIZE),
            };

            ShotPermissions = new ArcShotPermissions()
            {
                CanShootPrimaryWeapon = true,
            };
        }
    }
}
