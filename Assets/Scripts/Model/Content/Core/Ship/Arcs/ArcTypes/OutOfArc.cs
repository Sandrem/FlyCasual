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

            Limits = new Dictionary<Vector3, float>();
            Edges = new List<Vector3>()
            {
                new Vector3(-shipBase.HALF_OF_SHIPSTAND_SIZE, 0, 0),
                new Vector3( shipBase.HALF_OF_SHIPSTAND_SIZE, 0, 0),
                new Vector3(-shipBase.HALF_OF_SHIPSTAND_SIZE, 0, -shipBase.SHIPSTAND_SIZE),
                new Vector3( shipBase.HALF_OF_SHIPSTAND_SIZE, 0, -shipBase.SHIPSTAND_SIZE)
            };

            ShotPermissions = new ArcShotPermissions(); //Cannot shoot out of arc by default
        }
    }
}
