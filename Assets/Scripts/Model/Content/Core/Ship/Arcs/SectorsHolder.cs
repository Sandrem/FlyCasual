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
        public List<GenericArc> Sectors { get { return Arcs.Where(a => a is ArcSector).ToList(); } }

        public SectorsHolder(GenericShip hostShip) : base(hostShip)
        {
            Arcs = new List<GenericArc>()
            {
                new ArcBullseye(hostShip.ShipBase),
                new ArcSector(hostShip.ShipBase) { Facing = ArcFacing.Front },
                new ArcSector(hostShip.ShipBase) { Facing = ArcFacing.Left },
                new ArcSector(hostShip.ShipBase) { Facing = ArcFacing.Right },
                new ArcSector(hostShip.ShipBase) { Facing = ArcFacing.Rear },
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
    }
}
