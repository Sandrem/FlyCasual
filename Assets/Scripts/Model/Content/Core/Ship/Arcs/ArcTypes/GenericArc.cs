using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;

namespace Arcs
{
    public enum ArcType
    {
        None,
        Front,
        Rear,
        Left,
        Right,
        FullFront,
        FullRear,
        SingleTurret,
        DoubleTurret,
        Bullseye,
        TurretPrimaryWeapon,
        SpecialGhost
    }

    public enum ArcFacing
    {
        None,
        Front,
        Left,
        Right,
        Rear,
        FullFront,
        FullRear,
        Bullseye
    }
    
    public class GenericArc
    {
        public GenericShipBase ShipBase;

        public ArcType ArcType;
        public virtual ArcFacing Facing { get; set; }

        public virtual Dictionary<Vector3, float> Limits { get; set; }
        public virtual List<Vector3> Edges { get; set; }

        public bool WasUsedForAttackThisRound { get; set; }

        public GenericArc(GenericShipBase shipBase)
        {
            ShipBase = shipBase;
        }

        public virtual void RemoveArc()
        {
            ShipBase.Host.ArcsInfo.Arcs.Remove(this);
        }
    }
}
