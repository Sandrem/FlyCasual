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

        public ArcType ArcType;
        public virtual ArcFacing Facing { get; set; }

        public virtual Dictionary<Vector3, float> Limits { get; set; }
        public virtual List<Vector3> Edges { get; set; }

        public ArcShotPermissions ShotPermissions;

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
