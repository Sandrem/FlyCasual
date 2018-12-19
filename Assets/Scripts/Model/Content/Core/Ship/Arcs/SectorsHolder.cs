using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arcs;
using BoardTools;

namespace Ship
{
    public class SectorsHolder : ArcsHolder
    {
        public List<GenericArc> Sectors { get { return Arcs.Where(a => a is ArcPrimary || a is ArcLeft || a is ArcRight || a is ArcRear).ToList(); } }

        public SectorsHolder(GenericShip hostShip) : base(hostShip)
        {
            Arcs = new List<GenericArc>()
            {
                new ArcBullseye(hostShip.ShipBase),
                new ArcPrimary(hostShip.ShipBase),
                new ArcRear(hostShip.ShipBase),
                new ArcLeft(hostShip.ShipBase),
                new ArcRight(hostShip.ShipBase),
                new ArcFullFront(hostShip.ShipBase),
                new ArcFullRear(hostShip.ShipBase)
            };
        }

        public Dictionary<ArcFacing, List<GenericShip>> GetEnemiesInAllSectors()
        {
            Dictionary<ArcFacing, List<GenericShip>> EnemiesInAllSectors = new Dictionary<ArcFacing, List<GenericShip>>();

            foreach (GenericArc sector in Sectors)
            {
                EnemiesInAllSectors.Add(sector.Facing, new List<GenericShip>());
                foreach (GenericShip enemyShip in HostShip.Owner.AnotherPlayer.Ships.Values)
                {
                    ShotInfoArc sectorInfo = new ShotInfoArc(HostShip, enemyShip, sector);
                    if (sectorInfo.InArc && sectorInfo.Range != 0)
                    {
                        EnemiesInAllSectors[sector.Facing].Add(enemyShip);
                    }
                }
            }

            return EnemiesInAllSectors;
        }

        public bool IsShipInSector(GenericShip anotherShip, ArcType arcType)
        {
            GenericArc arc = Arcs.First(n => n.ArcType == arcType);
            ShotInfoArc arcInfo = new ShotInfoArc(HostShip, anotherShip, arc);
            return arcInfo.IsShotAvailable;
        }

        public int RangeToShipBySector(GenericShip anotherShip, ArcType arcType)
        {
            GenericArc arc = Arcs.First(n => n.ArcType == arcType);
            ShotInfoArc arcInfo = new ShotInfoArc(HostShip, anotherShip, arc);
            return arcInfo.Range;
        }
    }
}
