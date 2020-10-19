using BoardTools;
using Remote;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arcs
{
    public class ArcsHolder
    {
        protected readonly GenericShip HostShip;
        public List<GenericArc> Arcs { get; set; }

        public ArcsHolder(GenericShip hostShip)
        {
            HostShip = hostShip;

            Arcs = new List<GenericArc>();
            if (!(HostShip is GenericRemote)) Arcs.Add(new OutOfArc(hostShip.ShipBase));
        }

        public T GetArc<T>() where T : GenericArc
        {
            return (T)Arcs.FirstOrDefault(n => n.GetType() == typeof(T));
        }

        public bool HasArc(ArcType arcType)
        {
            return HostShip.ShipInfo.ArcInfo.Arcs.Any(a => a.ArcType == arcType);
        }
        public bool HasShipInTurretArc(GenericShip targetShip)
        {
            var turretArcs = HostShip.ArcsInfo.Arcs.Where(arc => arc.IsTurretArc);
            return turretArcs.Any(arc => new ShotInfoArc(HostShip, targetShip, arc).InArc);
        }

    }

}
