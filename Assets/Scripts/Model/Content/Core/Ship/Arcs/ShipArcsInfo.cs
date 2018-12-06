using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arcs;

namespace Ship
{
    public class ShipArcInfo
    {
        public ArcType ArcType { get; private set; }
        public int Firepower { get; set; }

        public ShipArcInfo(ArcType arcType, int firepower)
        {
            ArcType = arcType;
            Firepower = firepower;
        }
    }

    public class ShipArcsInfo
    {
        public List<ShipArcInfo> Arcs { get; private set; }

        public ShipArcsInfo(params ShipArcInfo[] arcs)
        {
            Arcs = arcs.ToList();
        }

        public ShipArcsInfo(ArcType arcType, int firepower)
        {
            Arcs = new List<ShipArcInfo>() { new ShipArcInfo(arcType, firepower) };
        }
    }
}
