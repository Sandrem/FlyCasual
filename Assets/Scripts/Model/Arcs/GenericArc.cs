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

        public GenericArc()
        {

        }

        public virtual bool InAttackAngle(GenericShip targetShip)
        {
            return CheckAngle(targetShip, attackAngles);
        }

        public virtual bool InArc(GenericShip targetShip)
        {
            return CheckAngle(targetShip, attackAngles);
        }

        public virtual bool InPrimaryArc(GenericShip targetShip)
        {
            return CheckAngle(targetShip, primaryArcAngle);
        }

        private bool CheckAngle(GenericShip targetShip, List<ArcInfo> requiredAngles)
        {
            Vector3 vectorFacing = Host.GetFrontFacing();

            foreach (var objThis in Host.GetStandFrontPoins())
            {
                foreach (var objAnother in targetShip.GetStandPoints())
                {
                    Vector3 vectorToTarget = objAnother.Value - objThis.Value;
                    float angle = Vector3.SignedAngle(vectorToTarget, vectorFacing, Vector3.up);

                    foreach (var arcInfo in requiredAngles)
                    {
                        if (angle >= arcInfo.MinAngle && angle <= arcInfo.MaxAngle)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

    }
}
