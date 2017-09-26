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
        ArcMobile
    }

    public class ArcInfo
    {
        public GenericShipBase ShipBase;
        public float MinAngle;
        public float MaxAngle;
        public ArcFacing Facing;
        public bool CanShoot = true;

        public virtual Dictionary<string, Vector3> GetArcPoints()
        {
            Dictionary<string, Vector3> results = new Dictionary<string, Vector3>();

            switch (Facing)
            {
                case ArcFacing.Front:
                    results = ShipBase.GetStandFrontPoints();
                    break;
                case ArcFacing.Left:
                    results = ShipBase.GetStandLeftPoints();
                    break;
                case ArcFacing.Right:
                    results = ShipBase.GetStandRightPoints();
                    break;
                case ArcFacing.Rear:
                    results = ShipBase.GetStandBackPoints();
                    break;
                case ArcFacing.Forward180:
                    results = ShipBase.GetStandFront180Points();
                    break;
                default:
                    break;
            }

            return results;
        }
    }

    public enum ArcFacing
    {
        Front,
        Left,
        Right,
        Rear,
        Forward180
    }

    public class GenericArc
    {
        public GenericShip Host;

        protected readonly ArcInfo primaryArc;
        protected List<ArcInfo> ArcsList;

        public bool CanShootOutsideArc { get; protected set; }

        public GenericArc(GenericShip host)
        {
            Host = host;

            primaryArc = new ArcInfo()
            {
                ShipBase = Host.ShipBase,
                MaxAngle = -40f,
                MinAngle = 40f,
                Facing = ArcFacing.Front
            };

            ArcsList = new List<ArcInfo>
            {
                primaryArc
            };
        }

        public virtual bool InAttackAngle(float angle)
        {
            return CheckAngle(angle, ArcsList);
        }

        public virtual bool InArc(float angle)
        {
            return CheckAngle(angle, ArcsList);
        }

        public virtual bool InPrimaryArc(float angle)
        {
            return CheckAngle(angle, new List<ArcInfo>() { primaryArc });
        }

        private bool CheckAngle(float angle, List<ArcInfo> arcList)
        {

            foreach (var arcInfo in arcList)
            {
                if (arcInfo.Facing != ArcFacing.Rear)
                {
                    if (angle >= arcInfo.MinAngle && angle <= arcInfo.MaxAngle)
                    {
                        return true;
                    }
                }
                else
                {
                    if (angle <= arcInfo.MinAngle || angle >= arcInfo.MaxAngle)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public virtual Dictionary<string, Vector3> GetArcsPoints()
        {
            Dictionary<string, Vector3> result = new Dictionary<string, Vector3>();
            foreach (var arc in ArcsList)
            {
                foreach (var point in arc.GetArcPoints())
                {
                    if (!result.ContainsKey(point.Key))
                    {
                        result.Add(point.Key, point.Value);
                    }
                }
            }
            return result;
        }

    }
}
