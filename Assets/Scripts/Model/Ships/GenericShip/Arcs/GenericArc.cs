using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;

namespace Arcs
{
    public enum BaseArcsType
    {
        ArcDefault,
        ArcRear,
        Arc180,
        Arc360,
        ArcMobile,
        ArcBullseye,
        ArcGhost
    }

    public enum ArcTypes
    {
        None,
        Primary,
        RearAux,
        Arc180,
        Mobile,
        Bullseye,
        Special
    }

    public enum ArcFacing
    {
        None,
        Front,
        Left,
        Right,
        Rear,
        Forward180,
        Bullseye
    }

    public class ArcShotPermissions
    {
        public bool CanShootPrimaryWeapon;
        public bool CanShootTurret;
        public bool CanShootTorpedoes;
        public bool CanShootMissiles;
        public bool CanShootCannon;
    }

    public class GenericArc
    {
        public GenericShipBase ShipBase;
        public ArcTypes ArcType;
        public ArcFacing Facing;
        public float MinAngle;
        public float MaxAngle;
        public ArcShotPermissions ShotPermissions;

        public GenericArc(GenericShipBase shipBase)
        {
            ShipBase = shipBase;
        }
    }

    public class ArcsHolder
    {
        public List<GenericArc> Arcs { get; private set; }

        public ArcsHolder(GenericShip host)
        {
            Arcs = new List<GenericArc>
            {
                new ArcPrimary(host.ShipBase),
                new OutOfArc(host.ShipBase)
            };
        }

        public T GetArc<T>() where T : GenericArc
        {
            return (T)Arcs.FirstOrDefault(n => n.GetType() == typeof(T));
        }

    }
}
