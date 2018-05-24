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
        ArcSpecial180,
        Arc360,
        ArcMobile,
        ArcBullseye,
        ArcSpecialGhost
    }

    public enum ArcTypes
    {
        None,
        Primary,
        RearAux,
        Special180,
        Mobile,
        Bullseye,
        SpecialGhost
    }

    public enum ArcFacing
    {
        None,
        Forward,
        Left,
        Right,
        Rear,
        Front180,
        Rear180,
        Bullseye
    }

    public class ArcShotPermissions
    {
        public bool CanShootPrimaryWeapon;
        public bool CanShootTurret;
        public bool CanShootTorpedoes;
        public bool CanShootMissiles;
        public bool CanShootCannon;

        public bool CanShootByWeaponType(WeaponTypes weaponType)
        {
            switch (weaponType)
            {
                case WeaponTypes.PrimaryWeapon:
                    return CanShootPrimaryWeapon;
                case WeaponTypes.Torpedo:
                    return CanShootTorpedoes;
                case WeaponTypes.Missile:
                    return CanShootMissiles;
                case WeaponTypes.Cannon:
                    return CanShootCannon;
                case WeaponTypes.Turret:
                    return CanShootTurret;
                default:
                    break;
            }

            return false;
        }
    }

    public class GenericArc
    {
        public GenericShipBase ShipBase;

        public ArcTypes ArcType;
        public virtual ArcFacing Facing { get; set; }

        public virtual Dictionary<Vector3, float> Limits { get; set; }
        public virtual List<Vector3> Edges { get; set; }

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
