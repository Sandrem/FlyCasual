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
        Primary,
        RearAux,
        Arc180,
        ArcMobile,
        ArcBullseye,
        ArcSpecial
    }

    public enum ArcFacing
    {
        Front,
        Left,
        Right,
        Rear,
        Forward180,
        Bullseye
    }

    public class ArcShotPermissions
    {
        public bool CanShootPrimaryWeapon = true;
        public bool CanShootTurret = true;
        public bool CanShootTorpedoes = false;
        public bool CanShootMissiles = false;
        public bool CanShootCannon = false;
    }

    public class GenericArc
    {
        public GenericShipBase ShipBase;
        public ArcTypes ArcType;
        public ArcFacing Facing;
        public float MinAngle;
        public float MaxAngle;
        public ArcShotPermissions ShotPermissions = new ArcShotPermissions();
    }

    public class ArcsHolder
    {
        public GenericShip Host;

        public List<GenericArc> Arcs { get; private set; }

        public ArcsHolder(GenericShip host)
        {
            Host = host;

            /*primaryArc = new GenericArc()
            {
                ShipBase = Host.ShipBase,
                MinAngle = -40f,
                MaxAngle = 40f,
                Facing = ArcFacing.Front,
                ShotPermissions = new ArcShotPermissions()
                {
                    CanShootPrimaryWeapon = true,
                    CanShootTorpedoes = true,
                    CanShootMissiles = true,
                    CanShootCannon = true,
                    CanShootTurret = true
                }
            };*/

            /*ArcsList = new List<GenericArc>
            {
                primaryArc
            };*/
        }

        public T GetArc<T>() where T : GenericArc
        {
            return (T)Arcs.FirstOrDefault(n => n.GetType() == typeof(T));
        }

    }
}
