using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;

namespace Arcs
{
    public class ArcInfo
    {
        public float MinAngle;
        public float MaxAngle;

        public ArcInfo(float minAngle, float maxAngle)
        {
            MinAngle = minAngle;
            MaxAngle = maxAngle;
        }
    }

    public enum ArcFacing
    {
        Forward,
        Left,
        Right,
        Rear
    }

    public class GenericArc
    {

        public GenericShip Host;

        protected readonly List<ArcInfo> primaryArcAngle = new List<ArcInfo> { new ArcInfo(-40f, 40f) };
        protected List<ArcInfo> attackAngles = new List<ArcInfo> { new ArcInfo(-40f, 40f) };

        public bool CanShootOutsideArc { get; protected set; }

        public GenericArc(GenericShip host)
        {
            Host = host;
        }

        public virtual bool InAttackAngle(float angle)
        {
            return CheckAngle(angle, attackAngles);
        }

        public virtual bool InArc(float angle)
        {
            return CheckAngle(angle, attackAngles);
        }

        public virtual bool InPrimaryArc(float angle)
        {
            return CheckAngle(angle, primaryArcAngle);
        }

        private bool CheckAngle(float angle, List<ArcInfo> requiredAngles)
        {

            foreach (var arcInfo in requiredAngles)
            {
                if (angle >= arcInfo.MinAngle && angle <= arcInfo.MaxAngle)
                {
                    return true;
                }
            }

            return false;
        }

    }
}
