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
        public string Name { get; private set; }

        public ShipArcInfo(ArcType arcType, int firepower = -1)
        {
            ArcType = arcType;
            Firepower = firepower;
            Name = GetArcName(arcType);
        }

        private string GetArcName(ArcType arcType)
        {
            string result = "";

            switch (arcType)
            {
                case ArcType.Front:
                    result = "Front";
                    break;
                case ArcType.Rear:
                    result = "Rear";
                    break;
                case ArcType.FullFront:
                    result = "Full Front";
                    break;
                case ArcType.FullRear:
                    result = "Full Rear";
                    break;
                case ArcType.SingleTurret:
                    result = "Turret";
                    break;
                case ArcType.DoubleTurret:
                    result = "Turret";
                    break;
                case ArcType.Bullseye:
                    result = "Bullseye";
                    break;
                case ArcType.TurretPrimaryWeapon:
                    result = "Out of Arc";
                    break;
                case ArcType.SpecialGhost:
                    result = "Special";
                    break;
                default:
                    break;
            }

            return result;
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
