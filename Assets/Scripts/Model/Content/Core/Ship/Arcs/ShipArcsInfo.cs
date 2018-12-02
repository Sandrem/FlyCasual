using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arcs;

namespace Ship
{
    public class ShipArcsInfo
    {
        public ArcType ArcType { get; private set; }
        public int Firepower { get; set; }

        public ShipArcsInfo(ArcType arcType, int firepower)
        {
            ArcType = arcType;
            Firepower = firepower;
        }
    }
}
