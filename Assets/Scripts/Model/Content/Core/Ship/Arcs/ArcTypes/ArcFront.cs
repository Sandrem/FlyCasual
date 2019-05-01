using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;

namespace Arcs
{
    public class ArcFront : GenericArc
    {
        public ArcFront(GenericShipBase shipBase) : base(shipBase)
        {
            ArcType = ArcType.Front;
            Facing = ArcFacing.Front;

            Limits = new Dictionary<Vector3, float>()
            {
                { new Vector3(-shipBase.HALF_OF_FIRINGARC_SIZE, 0, 0), -40f },
                { new Vector3( shipBase.HALF_OF_FIRINGARC_SIZE, 0, 0),  40f }
            };

            Edges = new List<Vector3>()
            {
                new Vector3(-shipBase.HALF_OF_FIRINGARC_SIZE, 0, 0),
                new Vector3( shipBase.HALF_OF_FIRINGARC_SIZE, 0, 0),
            };
        }
    }
}
