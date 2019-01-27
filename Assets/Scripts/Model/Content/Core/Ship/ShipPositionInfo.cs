using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ship
{
    public class ShipPositionInfo
    {
        public Vector3 Position { get; private set; }
        public Vector3 Angles { get; private set; }

        public ShipPositionInfo(Vector3 position, Vector3 angles)
        {
            Position = position;
            Angles = angles;
        }
    }
}
