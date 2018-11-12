using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arcs;

namespace Ship
{
    public class ShipArcsInfo
    {
        public ArcTypes ArcType { get; private set; }
        public int Firepower { get; private set; }

        public ShipArcsInfo(ArcTypes arcType, int firepower)
        {
            ArcType = arcType;
            Firepower = firepower;
        }
    }
}
