using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ship
{
    public struct ShipPositionInfo
    {
        public Vector3 Position { get; private set; }
        public Vector3 Angles { get; private set; }

        public ShipPositionInfo(Vector3 position, Vector3 angles)
        {
            Position = position;
            Angles = angles;
        }

        public static bool operator ==(ShipPositionInfo x, ShipPositionInfo y)
        {
            return x.Position == y.Position && x.Angles == y.Angles;
        }

        public static bool operator !=(ShipPositionInfo x, ShipPositionInfo y)
        {
            return x.Position != y.Position || x.Angles != y.Angles;
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode() ^ Angles.GetHashCode();
        }

        public override bool Equals(System.Object obj)
        {
            return obj is ShipPositionInfo && this == (ShipPositionInfo)obj;
        }
    }
}
