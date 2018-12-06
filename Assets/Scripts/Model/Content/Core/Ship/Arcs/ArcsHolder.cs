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
            Arcs = new List<GenericArc>
            {
                new OutOfArc(hostShip.ShipBase)
            };
        }

        public T GetArc<T>() where T : GenericArc
        {
            return (T)Arcs.FirstOrDefault(n => n.GetType() == typeof(T));
        }

        public bool HasArc(ArcType arcType)
        {
            return HostShip.ShipInfo.ArcInfo.Arcs.Any(a => a.ArcType == arcType);
        }

    }

}
